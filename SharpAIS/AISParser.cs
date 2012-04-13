using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SharpAIS
{
	public class AISParser
	{
		private static Dictionary<string, string> buffer = new Dictionary<string, string>();
		private static Hashtable patterns = new Hashtable();

		static AISParser()
		{
			// 1: Scheduled Position Report
			// Based on: http://www.navcen.uscg.gov/?pageName=AISMessagesA
			patterns.Add(
				 "^" + IntegerToBinary(1, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "NavigationalStatus:int:4",
			 "RateOfTurn:sint:8",
			 "SpeedOverGround:int:10",
			 "PositionAccuracy:int:1",
			 "Longitude:double:28:600000",
			 "Latitude:double:27:600000",
			 "CourseOverGround:double:12:10",
			 "TrueHeading:int:9",
			 "TimeStamp:int:6",
			 "SpecialManeuvreIndicator:int:2",
			 "Spare:int:3",
			 "RAIMFlag:int:1",
			 "SyncState:int:2",
			 "SlotTimeOut:int:3",
			 "SubMessage:data:14"
		 });

			// 2: Assigned Scheduled Position Report
			// Based on: http://www.navcen.uscg.gov/?pageName=AISMessagesA
			patterns.Add(
				 "^" + IntegerToBinary(2, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "NavigationalStatus:int:4",
			 "RateOfTurn:sint:8",
			 "SpeedOverGround:int:10",
			 "PositionAccuracy:int:1",
			 "Longitude:double:28:600000",
			 "Latitude:double:27:600000",
			 "CourseOverGround:double:12:10",
			 "TrueHeading:int:9",
			 "TimeStamp:int:6",
			 "SpecialManeuvreIndicator:int:2",
			 "Spare:int:3",
			 "RAIMFlag:int:1",
			 "SyncState:int:2",
			 "SlotTimeOut:int:3",
			 "SubMessage:data:14"
		 });

			// 3: Special Position Report, Response to Interrogation
			// Based on: http://www.navcen.uscg.gov/?pageName=AISMessagesA
			patterns.Add(
				 "^" + IntegerToBinary(3, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "NavigationalStatus:int:4",
			 "RateOfTurn:sint:8",
			 "SpeedOverGround:int:10",
			 "PositionAccuracy:int:1",
			 "Longitude:double:28:600000",
			 "Latitude:double:27:600000",
			 "CourseOverGround:double:12:10",
			 "TrueHeading:int:9",
			 "TimeStamp:int:6",
			 "SpecialManeuvreIndicator:int:2",
			 "Spare:int:3",
			 "RAIMFlag:int:1",
			 "SyncState:int:2",
			 "SlotTimeOut:int:3",
			 "SubMessage:data:14"
		 });

			// 4: Base Station Info (ok)
			// Based on: http://www.navcen.uscg.gov/pdf/AIS/ITU-R_M1371-3_AIS_Msg_4.pdf
			patterns.Add(
				 "^" + IntegerToBinary(4, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "UTCYear:int:14",
			 "UTCMonth:int:4",
			 "UTCDay:int:5",
			 "UTCHour:int:5",
			 "UTCMinute:int:6",
			 "UTCSecond:int:6",
			 "PositionAccuracy:int:1",
			 "Longitude:double:28:600000",
			 "Latitude:double:27:600000",
			 "FixingDeviceType:int:4",
			 "Spare:int:10",
			 "RAIMFlag:int:1",
			 "SyncState:int:2",   // CommunicationState (SOTDMA)
			 "SlotTimeOut:int:3",
			 "SubMessage:int:14"
		 });

			// 5: Ship Static and Voyage related data - ^000101.{2}.{30}00.*
			// (DSI=0) Ship Static and Voyage Related Data
			// Based on: http://www.navcen.uscg.gov/?pageName=AISMessagesAStatic
			patterns.Add(
				 "^" + IntegerToBinary(5, 6) + ".{2}.{30}00.*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "DSI:int:2",
			 "IMONumber:int:30",
			 "CallSign:string:42",
			 "Name:string:120",
			 "TypeOfShipAndCargoType:int:8",
			 "LengthFore:int:9",
			 "LengthAft:int:9",
			 "WidthPort:int:6",
			 "WidthStarboard:int:6",
			 "TypeOfElectronicPositionFixingDevice:int:4",
			 "ETAMonth:int:4",
			 "ETADay:int:5",
			 "ETAHour:int:5",
			 "ETAMinute:int:6",
			 "MaximumPresentStaticDraught:int:8",
			 "Destination:string:120",
			 "DTE:int:1",
			 "Spare:int:1"
		 });

			// 5: Ship Static and Voyage related data - ^000101.{2}.{30}01.*
			// (DSI=1) Data Set for Extended Ship Static and Voyage Related Data

			// 5: Ship Static and Voyage related data - ^000101.{2}.{30}10.*
			// (DSI=2) Data Set for Aids-to-Navigation Data

			// 5: Ship Static and Voyage related data - ^000101.{2}.{30}11.*
			// (DSI=3) Data set for Regional Ship Static and Voyage Related Data

			// 6: Addressed Binary Message (ok)
			patterns.Add(
				 "^" + IntegerToBinary(6, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "SequenceNumber:int:2",
			 "DestinationID:int:30",
			 "Retransmit:int:1",
			 "Spare:int:1",
			 "DesignatedAreaCode:int:10",
			 "FunctionIdentifier:int:6",
			 "ApplicationData:data:0,920"
		 });


			// 7: Binary Acknowledge (w/ 4 destinations) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(7, 6) + ".{162}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "SequenceNumberforID1:int:2",
			 "DestinationID2:int:30",
			 "SequenceNumberforID2:int:2",
			 "DestinationID3:int:30",
			 "SequenceNumberforID3:int:2",
			 "DestinationID4:int:30",
			 "SequenceNumberforID4:int:2"
		 });


			// 7: Binary Acknowledge (w/ 3 destinations) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(7, 6) + ".{130}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "SequenceNumberforID1:int:2",
			 "DestinationID2:int:30",
			 "SequenceNumberforID2:int:2",
			 "DestinationID3:int:30",
			 "SequenceNumberforID3:int:2"
		 });


			// 7: Binary Acknowledge (w/ 2 destinations) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(7, 6) + ".{98}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "SequenceNumberforID1:int:2",
			 "DestinationID2:int:30",
			 "SequenceNumberforID2:int:2"
		 });


			// 7: Binary Acknowledge (w/ 1 destinations) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(7, 6) + ".{66}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "SequenceNumberforID1:int:2"
		 });

			// 8: Binary Broadcast Message (ok)
			patterns.Add(
				 "^" + IntegerToBinary(8, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DesignatedAreaCode:int:10",
			 "FunctionIdentifier:int:6",
			 "ApplicationData:data:0,952"
		 });


			// 9: Special Position Report for SAR (ok)
			patterns.Add(
				 "^" + IntegerToBinary(9, 6) + ".*", new string[] {
			 "MessageId:int:6",
			 "DTE:int:1",
			 "DataIndicator:int:1",
			 "UserID:int:30",
			 "Altitude:int:12",
			 "SpeedOverGround:int:10",
			 "PositionAccuracy:int:1",
			 "Longitude:double:28:600000",
			 "Latitude:double:27:600000",
			 "CourseOverGround:double:12:10",
			 "TimeStamp:int:6",
			 "RepeatIndicator:int:2",
			 "RegionalApplication:int:13",
			 "Spare:int:1",
			 "SyncState:int:2",   // CommunicationState (SOTDMA)
			 "SlotTimeOut:int:2",
			 "SubMessage:int:14"
		 });


			// 10: UTC and Date Inquiry (ok)
			patterns.Add(
				 "^" + IntegerToBinary(10, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID:int:30",
			 "Spare2:int:2"
		 });


			// 11: UTC and Date Response (ok)
			patterns.Add(
				 "^" + IntegerToBinary(11, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "UTCYear:int:14",
			 "UTCMonth:int:4",
			 "UTCDay:int:5",
			 "UTCHour:int:5",
			 "UTCMinute:int:6",
			 "UTCSecond:int:6",
			 "PositionAccuracy:int:1",
			 "Longitude:double:28:600000",
			 "Latitude:double:27:600000",
			 "FixingDeviceType:int:4",
			 "Spare:int:12",
			 "SyncState:int:2",   // CommunicationState (SOTDMA)
			 "SlotTimeOut:int:2",
			 "SubMessage:int:14"
		 });


			// 12: Addressed Safety Related Message (ok)
			patterns.Add(
				 "^" + IntegerToBinary(12, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "SequenceNumber:int:2",
			 "DestinationID:int:30",
			 "RetransmitFlag:int:1",
			 "Spare:int:1",
			 "SafetyRelatedText:string:0,936"
		 });


			// 13: Safety Related Acknowledge (w/ 4 destinations) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(13, 6) + ".{162}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "SequenceNumberforID1:int:2",
			 "DestinationID2:int:30",
			 "SequenceNumberforID2:int:2",
			 "DestinationID3:int:30",
			 "SequenceNumberforID3:int:2",
			 "DestinationID4:int:30",
			 "SequenceNumberforID4:int:2"
		 });


			// 13: Safety Related Acknowledge (w/ 3 destinations) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(13, 6) + ".{130}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "SequenceNumberforID1:int:2",
			 "DestinationID2:int:30",
			 "SequenceNumberforID2:int:2",
			 "DestinationID3:int:30",
			 "SequenceNumberforID3:int:2"
		 });


			// 13: Safety Related Acknowledge (w/ 2 destinations) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(13, 6) + ".{98}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "SequenceNumberforID1:int:2",
			 "DestinationID2:int:30",
			 "SequenceNumberforID2:int:2"
		 });


			// 13: Safety Related Acknowledge (w/ 1 destinations) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(13, 6) + ".{66}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "SequenceNumberforID1:int:2"
		 });

			// 14: Safety Related Broadcast Message (ok)
			patterns.Add(
				 "^" + IntegerToBinary(14, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "SafetyRelatedText:string:0,968"
		 });


			// 15: Interrogation, 2 Stations (ok)
			// DestinationID1 2 messages, DestinationID2 1 message (ok)
			// DestinationID1 1 messages, DestinationID2 1 message (ok)
			patterns.Add(
				 "^" + IntegerToBinary(15, 6) + ".{160}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "MessageID1.1:int:6",
			 "SlotOffset1.1:int:12",
			 "DataSetIndicator1.1:int:2",
			 "MessageID1.2:int:6",
			 "SlotOffset1.2:int:12",
			 "DataSetIndicator1.2:int:2",
			 "DestinationID2:int:30",
			 "MessageID2.1:int:6",
			 "SlotOffset2.1:int:12",
			 "DataSetIndicator2.1:int:2"
		 });

			// 15: Interrogation, 1 Station (ok)
			// DestinationID1 2 messages (ok)
			// DestinationID1 1 messages (ok)
			patterns.Add(
				 "^" + IntegerToBinary(15, 6) + ".{108}", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "MessageID1.1:int:6",
			 "SlotOffset1.1:int:12",
			 "DataSetIndicator1.1:int:2",
			 "MessageID1.2:int:6",
			 "SlotOffset1.2:int:12"
			});

			// 15: Interrogation, pattern 1
			patterns.Add(
				 "^" + IntegerToBinary(15, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceID:int:30",
			 "Spare:int:2",
			 "DestinationID1:int:30",
			 "MessageID1.1:int:6",
			 "SlotOffset1.1:int:12"
		 });

			// 16: Assignment Mode Command
			patterns.Add(
				 "^" + IntegerToBinary(16, 6) + ".*", new string[] {
			 "MessageType:int:6",
			 "Repeat:int:2",
			 "sourceMmsi:int:30",
			 "spare:int:2",
			 "destinationMmsi1:int:30",
			 "offset1:int:12",
			 "increment1:int:10",
			 "destinationMmsi2:int:30",
			 "offset2:int:12",
			 "increment2:int:10",
			 "spare:int:0,4"
		 });


			// 17: DGNSS Broadcast binary message
			patterns.Add(
				 "^" + IntegerToBinary(17, 6) + ".*", new string[] {
			 "MessageType:int:6",
			 "Repeat:int:2",
			 "sourceMmsi:int:30",
			 "spare:int:2",
			 "longitude:double:18:600",
			 "latitude:double:17:600",
			 "spare2:int:5",
			 "data:data:0,736"
		 });


			// 18: Class B Equipment Position Report (ok)
			// Based on: http://www.navcen.uscg.gov/enav/ais/AIS_Messages_B.htm
			patterns.Add(
				 "^" + IntegerToBinary(18, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "Spare:int:8",
			 "SpeedOverGround:double:10:10",
			 "PositionAccuracy:int:1",
			 "Longitude:double:28:600000",
			 "Latitude:double:27:600000",
			 "CourseOverGround:double:12:10",
			 "TrueHeading:int:9",
			 "Timestamp:int:6",
			 "Spare2:int:2",
			 "ClassBUnitFlag:int:1",
			 "ClassBDisplayFlag:int:1",
			 "ClassBDSCFlag:int:1",
			 "ClassBBandFlag:int:1",
			 "ClassBMessage22Flag:int:1",
			 "ModeFlag:int:1",
			 "RAIMFlag:int:1",
			 "CommunicationStateSelectorFlag:int:1",
			 "SyncState:int:2",   // CommunicationState (SOTDMA)
			 "SlotTimeOut:int:3",
			 "SubMessage:int:14"
		 });


			// 19: Class B Equipment Position Report, Response to Interrogation (ok)
			// Based on: http://www.navcen.uscg.gov/enav/ais/AIS_Messages_B.htm
			patterns.Add(
				 "^" + IntegerToBinary(19, 6) + ".*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "Spare:int:8",
			 "SpeedOverGround:double:10:10",
			 "PositionAccuracy:int:1",
			 "Longitude:double:28:600000",
			 "Latitude:double:27:600000",
			 "CourseOverGround:double:12:10",
			 "TrueHeading:int:9",
			 "Timestamp:int:6",
			 "Spare2:int:4",
			 "Name:string:120",
			 "TypeOfShipAndCargoType:int:8",
			 "LengthFore:int:9",
			 "LengthAft:int:9",
			 "WidthPort:int:6",
			 "WidthStarboard:int:6",
			 "TypeOfElectronicPositionFixingDevice:int:4",
			 "RAIMFlag:int:1",
			 "DTE:int:1",
			 "AssignedModeFlag:int:1",
			 "Spare3:int:4"
		 });


			// 20: Data Link Management Message (w/ 4 slot) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(20, 6) + ".{154}.*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceStationID:int:30",
			 "Spare:int:2",
			 "SlotOffsetNumber1:int:12",
			 "NumberOfSlots1:int:4",
			 "Timeout1:int:3",
			 "Increment1:int:11",
			 "SlotOffsetNumber2:int:12",
			 "NumberOfSlots2:int:4",
			 "Timeout2:int:3",
			 "Increment2:int:11",
			 "SlotOffsetNumber3:int:12",
			 "NumberOfSlots3:int:4",
			 "Timeout3:int:3",
			 "Increment3:int:11",
			 "SlotOffsetNumber4:int:12",
			 "NumberOfSlots4:int:4",
			 "Timeout4:int:3",
			 "Increment4:int:11"
		 });


			// 20: Data Link Management Message (w/ 3 slot) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(20, 6) + ".{124}.*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceStationID:int:30",
			 "Spare:int:2",
			 "SlotOffsetNumber1:int:12",
			 "NumberOfSlots1:int:4",
			 "Timeout1:int:3",
			 "Increment1:int:11",
			 "SlotOffsetNumber2:int:12",
			 "NumberOfSlots2:int:4",
			 "Timeout2:int:3",
			 "Increment2:int:11",
			 "SlotOffsetNumber3:int:12",
			 "NumberOfSlots3:int:4",
			 "Timeout3:int:3",
			 "Increment3:int:11"
		 });


			// 20: Data Link Management Message (w/ 2 slot) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(20, 6) + ".{94}.*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceStationID:int:30",
			 "Spare:int:2",
			 "SlotOffsetNumber1:int:12",
			 "NumberOfSlots1:int:4",
			 "Timeout1:int:3",
			 "Increment1:int:11",
			 "SlotOffsetNumber2:int:12",
			 "NumberOfSlots2:int:4",
			 "Timeout2:int:3",
			 "Increment2:int:11"
		 });


			// 20: Data Link Management Message (w/ 1 slot) (ok)
			patterns.Add(
				 "^" + IntegerToBinary(20, 6) + ".{64}.*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "SourceStationID:int:30",
			 "Spare:int:2",
			 "SlotOffsetNumber1:int:12",
			 "NumberOfSlots1:int:4",
			 "Timeout1:int:3",
			 "Increment1:int:11"
		 });


			// 21: Aids-to-Nav Report
			patterns.Add(
				 "^" + IntegerToBinary(21, 6) + ".*", new string[] {
			 "MessageType:int:6",
			 "Repeat:int:2",
			 "sourceMmsi:int:30",
			 "typeOfAid:int:5",
			 "name:string:120",
			 "positionAccuracy:int:1",
			 "longitude:double:28:600000",
			 "latitude:double:27:600000",
			 "lengthFore:int:9",
			 "lengthAft:int:9",
			 "widthPort:int:6",
			 "widthStarboard:int:6",
			 "fixDeviceType:int:4",
			 "timestamp:int:6",
			 "offPosition:int:1",
			 "localApplicationData:int:8",
			 "raimFlag:int:1",
			 "virtual:int:1",
			 "assignedMode:int:1",
			 "spare:int:1",
			 "nameExtension:string:0,84"
		 });


			// 22: Channel Management
			patterns.Add(
				 "^" + IntegerToBinary(22, 6) + ".*", new string[] {
			 "MessageType:int:6",
			 "Repeat:int:2",
			 "sourceMmsi:int:30",
			 "spare:int:2",
			 "channelA:int:12",
			 "channelB:int:12",
			 "transmitReceiveMode:4",
			 "power:int:1",
			 "longitude1:double:18:600",
			 "latitude1:double:17:600",
			 "longitude2:double:18:600",
			 "latitude2:double:17:600",
			 "addressOrBroadcast:int:1",
			 "channelABW:int:1",
			 "channelBBW:int:1",
			 "transmissionZoneSite:3",
			 "spare:int:23"		  
		 });


			// 24: Class B Static Data Report, Part A (ok)
			// Based on: http://www.navcen.uscg.gov/enav/ais/AIS_Messages_B.htm
			patterns.Add(
				 "^" + IntegerToBinary(24, 6) + ".{2}.{30}00.*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "PartNumber:int:2",
			 "Name:string:120"
		 });

			// 24: Class B Static Data Report, Part B (ok)
			// Based on: http://www.navcen.uscg.gov/enav/ais/AIS_Messages_B.htm
			patterns.Add(
				 "^" + IntegerToBinary(24, 6) + ".{2}.{30}01.*", new string[] {
			 "MessageID:int:6",
			 "RepeatIndicator:int:2",
			 "UserID:int:30",
			 "PartNumber:int:2",
			 "TypeOfShipAndCargoType:int:8",
			 "VendorID:string:42",
			 "CallSign:string:42",
			 "LengthFore:int:9",
			 "LengthAft:int:9",
			 "WidthPort:int:6",
			 "WidthStarboard:int:6",
			 "Spare:int:6"
		 });
		}

		public static Hashtable ParseSentence(string sentence)
		{
			string[] data = sentence.Split(',');

			//Debug.Assert(data[1] == "1");

			int msg_number = int.Parse(data[2]);
			int msg_part = int.Parse(data[1]);
			string msg_slot = data[3];

			string aisdata = EncodedDataToBinary(data[5]);
			int fillbits = int.Parse(data[6]);
			if (fillbits > 0)
			{
				for (int i = 0; i < fillbits; i++)
				{
					aisdata = aisdata + "0";
				}
			}

			if (msg_number == msg_part)
			{
				if (msg_part != 1)
				{
					//Debug.WriteLine(string.Format("Slot:[{0}] Part:{1}/{2}", msg_slot, msg_number, msg_part));
					if (buffer.ContainsKey(msg_slot))
					{
						//Debug.WriteLine(string.Format("Length:{0}", aisdata.Length));
						if (buffer[msg_slot].Length == (msg_number - 1) * 360)
							aisdata = buffer[msg_slot] + aisdata;
						else
							aisdata = string.Empty;

						buffer.Remove(msg_slot);
					}
					else
						aisdata = string.Empty;
				}

				//if (aisdata.StartsWith(i2b(1, 6)))
				//    System.Diagnostics.Debugger.Break();

				if (!string.IsNullOrEmpty(aisdata))
				{
					foreach (string key in patterns.Keys)
					{
						Regex regex = new Regex(key);
						if (regex.IsMatch(aisdata))
						{
							//if (aisdata.Length % 6 > 0)
							//    System.Diagnostics.Debugger.Break();

							return DecodeAISData(aisdata, (string[])patterns[key]);
						}
					}
				}
			}
			else
			{
				//Debug.WriteLine(string.Format("Slot:[{0}] Part:{1}/{2}", msg_slot, msg_number, msg_part));
				if (msg_number == 1)
				{
					if (buffer.ContainsKey(msg_slot))
					{
						Debug.WriteLine(string.Format("Slot:[{0}] deleted!", msg_slot));
						buffer.Remove(msg_slot);
					}

					buffer.Add(msg_slot, aisdata);
					Debug.WriteLine(string.Format("Length:{0}", aisdata.Length));
				}
				else
				{
					if (buffer.ContainsKey(msg_slot))
					{
						if (buffer[msg_slot].Length == (msg_number - 1) * 360)
							buffer[msg_slot] += aisdata;
						else
							buffer.Remove(msg_slot);
					}
				}
			}

			return null;
		}

		private static Hashtable DecodeAISData(string AISData, string[] attributes)
		{
			Hashtable returnData = new Hashtable();
			int currentPosition = 0;

			for (int i = 0; i < attributes.Length; i++)
			{
				string[] data = attributes[i].Split(':');
				string fieldName = data[0];
				string fieldType = data[1];
				int fieldMinLength = 0;
				int fieldMaxLength = 0;
				int divisor = 0;

				if (data[2].IndexOf(',') > -1)
				{
					string[] lengths = data[2].Split(',');
					fieldMinLength = int.Parse(lengths[0]);
					fieldMaxLength = int.Parse(lengths[1]);
				}
				else
				{
					fieldMinLength = int.Parse(data[2]);
					fieldMaxLength = fieldMinLength;
				}

				if (data.Length >= 4)
				{
					divisor = int.Parse(data[3]);
				}

				if (fieldMinLength == 0 && AISData.Length == currentPosition)
				{
					continue;
				}

				switch (fieldType)
				{
					case "int":
						returnData.Add(fieldName, Convert.ToInt32(AISData.Substring(currentPosition, fieldMinLength), 2));
						currentPosition += fieldMinLength;
						break;

					case "sint":
						returnData.Add(fieldName, BinToSignedInteger(AISData.Substring(currentPosition, fieldMinLength)));
						currentPosition += fieldMinLength;
						break;

					case "string":
						returnData.Add(fieldName, BinToString(AISData.Substring(currentPosition, fieldMinLength)));
						currentPosition += fieldMinLength;
						break;

					case "double":
						returnData.Add(fieldName, (double)(BinToSignedDouble(AISData.Substring(currentPosition, fieldMinLength)) / (double)divisor));
						currentPosition += fieldMinLength;
						break;

					case "data":
						if (fieldMinLength == fieldMaxLength)
						{
							returnData.Add(fieldName, AISData.Substring(currentPosition, fieldMaxLength));
							currentPosition += fieldMaxLength;
						}
						else
						{
							returnData.Add(fieldName, AISData.Substring(currentPosition));
						}
						break;

					default:
						break;
				}
			}
			return returnData;
		}

		private static string EncodedDataToBinary(string encodedData)
		{
			string decodedData = string.Empty;
			char[] encodedCharArray = encodedData.ToCharArray();

			for (int i = 0; i < encodedCharArray.Length; i++)
			{
				byte c = (byte)encodedData[i];
				if (c >= 48 && c < 88)
					c -= 48;
				else if (c >= 96 && c < 121)
					c -= 56;
				else
					c = 0;

				string decodedChar = Convert.ToString(c, 2);
				for (int j = decodedChar.Length; j < 6; j++)
				{
					decodedData += "0";
				}

				decodedData += decodedChar;
			}

			return decodedData;
		}

		private static string IntegerToBinary(int intergerValue, int outputLength)
		{
			string returnValue = Convert.ToString(intergerValue, 2);
			for (int i = returnValue.Length; i < outputLength; i++)
			{
				returnValue = "0" + returnValue;
			}

			return returnValue;
		}

		private static string BinToString(string encodedData)
		{
			string returnValue = "";

			for (int i = 0; i < encodedData.Length / 6; i++)
			{
				byte c = Convert.ToByte(encodedData.Substring(i * 6, 6), 2);

				if (c < 32 && c >= 0) //convert to 6-bit ASCII - control chars to uppercase latins
					c = (byte)(c + 64);

				if (c != 64)
					returnValue = returnValue + (char)c;
			}

			return returnValue;
		}

		private static double BinToSignedDouble(string encodedData)
		{
			double returnValue = (double)Convert.ToInt32(encodedData, 2);

			if (encodedData.StartsWith("1"))
				returnValue = returnValue - Math.Pow(2, encodedData.Length);

			return returnValue;
		}

		private static int BinToSignedInteger(string encodedData)
		{
			int returnValue = Convert.ToInt32(encodedData.Substring(1), 2);
			if (encodedData.StartsWith("1"))
				returnValue = (int)((double)returnValue - Math.Pow(2, encodedData.Length));

			return returnValue;
		}
	}
}
