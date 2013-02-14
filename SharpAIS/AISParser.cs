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
			// Type 1: Scheduled Position Report
			patterns.Add("^" + IntegerToBinary(1, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"NavigationalStatus:uint:4",
				"RateOfTurn:int:8",
				"SpeedOverGround:double:10:10",
				"PositionAccuracy:uint:1",
				"Lontitude:double:28:600000",
				"Lattitude:double:27:600000",
				"CourseOverGround:double:12:10",
				"TrueHeading:uint:9",
				"TimeStamp:uint:6",
				"ManeuverIndicator:uint:2",
				"Spare:uint:3",
				"RAIMFlag:uint:1",
				"SyncState:uint:2",
				"SlotTimeOut:uint:3",
				"SubMessage:data:14"
			});


			// Type 2: Assigned Scheduled Position Report
			patterns.Add("^" + IntegerToBinary(2, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"NavigationalStatus:uint:4",
				"RateOfTurn:int:8",
				"SpeedOverGround:double:10:10",
				"PositionAccuracy:uint:1",
				"Lontitude:double:28:600000",
				"Lattitude:double:27:600000",
				"CourseOverGround:double:12:10",
				"TrueHeading:uint:9",
				"TimeStamp:uint:6",
				"ManeuverIndicator:uint:2",
				"Spare:uint:3",
				"RAIMFlag:uint:1",
				"SyncState:uint:2",
				"SlotTimeOut:uint:3",
				"SubMessage:data:14"
			});


			// Type 3: Special Position Report, Response to Interrogation
			patterns.Add("^" + IntegerToBinary(3, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"NavigationalStatus:uint:4",
				"RateOfTurn:int:8",
				"SpeedOverGround:double:10:10",
				"PositionAccuracy:uint:1",
				"Lontitude:double:28:600000",
				"Lattitude:double:27:600000",
				"CourseOverGround:double:12:10",
				"TrueHeading:uint:9",
				"TimeStamp:uint:6",
				"ManeuverIndicator:uint:2",
				"Spare:uint:3",
				"RAIMFlag:uint:1",
				"SyncState:uint:2",
				"SlotTimeOut:uint:3",
				"SubMessage:data:14"
			});


			// Type 4: Base Station Info (ok)
			patterns.Add("^" + IntegerToBinary(4, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"Year:uint:14",
				"Month:uint:4",
				"Day:uint:5",
				"Hour:uint:5",
				"Minute:uint:6",
				"Second:uint:6",
				"PositionAccuracy:uint:1",
				"Lontitude:double:28:600000",
				"Lattitude:double:27:600000",
				"FixingDeviceType:uint:4",
				"TransmitionControl:uint:1",
				"Spare:uint:9",
				"RAIMFlag:uint:1",
				"SyncState:uint:2",   // CommunicationState (SOTDMA)
				"SlotTimeOut:uint:3",
				"SubMessage:data:14"
			});


			// Type 5: Ship Static and Voyage related data - ^000101.{2}.{30}00.*
			// (AIS Version=0) Ship Static and Voyage Related Data
			patterns.Add("^" + IntegerToBinary(5, 6) + ".{2}.{30}00.*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"AISVersion:uint:2",
				"IMONumber:uint:30",
				"CallSign:string:42",
				"VesselName:string:120",
				"ShipType:uint:8",
				"DimensionToBow:uint:9",
				"DimensionToStern:uint:9",
				"DimensionToPort:uint:6",
				"DimensionToStarboard:uint:6",
				"PositionFixType:uint:4",
				"ETAMonth:uint:4",
				"ETADay:uint:5",
				"ETAHour:uint:5",
				"ETAMinute:uint:6",
				"Draught:double:8:10",
				"Destination:string:120",
				"DTE:uint:1",
				"Spare:uint:1"
			});

			// Type 5: Ship Static and Voyage related data - ^000101.{2}.{30}01.*
			// (AIS Version=1) Data Set for Extended Ship Static and Voyage Related Data

			// Type 5: Ship Static and Voyage related data - ^000101.{2}.{30}10.*
			// (AIS Version=2) Data Set for Aids-to-Navigation Data

			// Type 5: Ship Static and Voyage related data - ^000101.{2}.{30}11.*
			// (AIS Version=3) Data set for Regional Ship Static and Voyage Related Data


			// Type 6: Addressed Binary Message (ok)
			patterns.Add("^" + IntegerToBinary(6, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"SequenceNumber:uint:2",
				"DestinationMMSI:uint:30",
				"RetransmitFlag:uint:1",
				"Spare:uint:1",
				"DesignatedAreaCode:uint:10",
				"FunctionalId:uint:6",
				"Data:data:0,920"
			});


			// Type 7: Binary Acknowledge (w/ 4 destinations) (ok)
			// Length: 168bit
			patterns.Add("^" + IntegerToBinary(7, 6) + ".{162}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"SequenceNumber1:uint:2",
				"DestinationMMSI2:uint:30",
				"SequenceNumber2:uint:2",
				"DestinationMMSI3:uint:30",
				"SequenceNumber3:uint:2",
				"DestinationMMSI4:uint:30",
				"SequenceNumber4:uint:2"
			});

			// Type 7: Binary Acknowledge (w/ 3 destinations) (ok)
			// Length: 136bit
			patterns.Add("^" + IntegerToBinary(7, 6) + ".{130,162}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"SequenceNumber1:uint:2",
				"DestinationMMSI2:uint:30",
				"SequenceNumber2:uint:2",
				"DestinationMMSI3:uint:30",
				"SequenceNumber3:uint:2"
			});

			// Type 7: Binary Acknowledge (w/ 2 destinations) (ok)
			// Length: 104bit
			patterns.Add("^" + IntegerToBinary(7, 6) + ".{98,130}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"SequenceNumber1:uint:2",
				"DestinationMMSI2:uint:30",
				"SequenceNumber2:uint:2"
			});

			// Type 7: Binary Acknowledge (w/ 1 destinations) (ok)
			// Length: 72bit
			patterns.Add("^" + IntegerToBinary(7, 6) + ".{66,98}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"SequenceNumber1:uint:2"
			});


			// Type 8: Binary Broadcast Message (ok)
			patterns.Add("^" + IntegerToBinary(8, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DesignatedAreaCode:uint:10",
				"FunctionId:uint:6",
				"Data:data:0,952"
			});


			// Type 9: Special Position Report for SAR (ok)
			patterns.Add("^" + IntegerToBinary(9, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"Altitude:uint:12",
				"SpeedOverGround:uint:10",
				"PositionAccuracy:uint:1",
				"Lontitude:double:28:600000",
				"Lattitude:double:27:600000",
				"CourseOverGround:double:12:10",
				"RegionalReserved:uint:8",
				"DTE:uint:1",
				"Spare:uint:3",
				"Assigned:uint:2",
				"RAIMFlag:uint:1",
				"SyncState:uint:2",   // CommunicationState (SOTDMA)
				"SlotTimeOut:uint:3",
				"SubMessage:data:14"
			});


			// Type 10: UTC and Date Inquiry (ok)
			patterns.Add("^" + IntegerToBinary(10, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare1:uint:2",
				"DestinationMMSI:uint:30",
				"Spare2:uint:2"
			});


			// Type 11: UTC and Date Response (ok)
			patterns.Add("^" + IntegerToBinary(11, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"Year:uint:14",
				"Month:uint:4",
				"Day:uint:5",
				"Hour:uint:5",
				"Minute:uint:6",
				"Second:uint:6",
				"PositionAccuracy:uint:1",
				"Lontitude:double:28:600000",
				"Lattitude:double:27:600000",
				"FixingDeviceType:uint:4",
				"TransmitionControl:uint:1",
				"Spare:uint:9",
				"RAIMFlag:uint:1",
				"SyncState:uint:2",   // CommunicationState (SOTDMA)
				"SlotTimeOut:uint:3",
				"SubMessage:data:14"
			});


			// Type 12: Addressed Safety Related Message (ok)
			patterns.Add("^" + IntegerToBinary(12, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"SequenceNumber:uint:2",
				"DestinationMMSI:uint:30",
				"RetransmitFlag:uint:1",
				"Spare:uint:1",
				"Text:string:0,936"
			});


			// Type 13: Safety Related Acknowledge (w/ 4 destinations) (ok)
			// Length: 168bit
			patterns.Add("^" + IntegerToBinary(13, 6) + ".{162}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"SequenceNumber1:uint:2",
				"DestinationMMSI2:uint:30",
				"SequenceNumber2:uint:2",
				"DestinationMMSI3:uint:30",
				"SequenceNumber3:uint:2",
				"DestinationMMSI4:uint:30",
				"SequenceNumber4:uint:2"
			});

			// Type 13: Safety Related Acknowledge (w/ 3 destinations) (ok)
			// Length: 136bit
			patterns.Add("^" + IntegerToBinary(13, 6) + ".{130,162}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"SequenceNumber1:uint:2",
				"DestinationMMSI2:uint:30",
				"SequenceNumber2:uint:2",
				"DestinationMMSI3:uint:30",
				"SequenceNumber3:uint:2",
			 });

			// Type 13: Safety Related Acknowledge (w/ 2 destinations) (ok)
			// Length: 104bit
			patterns.Add("^" + IntegerToBinary(13, 6) + ".{98,130}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"SequenceNumber1:uint:2",
				"DestinationMMSI2:uint:30",
				"SequenceNumber2:uint:2"
			 });

			// Type 13: Safety Related Acknowledge (w/ 1 destinations) (ok)
			// Length: 72bit
			patterns.Add("^" + IntegerToBinary(13, 6) + ".{66,98}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"SequenceNumber1:uint:2",
			});


			// Type 14: Safety Related Broadcast Message (ok)
			patterns.Add("^" + IntegerToBinary(14, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"Text:string:0,968"
			});


			// Type 15: Interrogation, 2 Stations (ok)
			// DestinationID1 2 messages, DestinationID2 1 message (ok)
			// DestinationID1 1 messages, DestinationID2 1 message (ok)
			// Length: 110bit
			patterns.Add("^" + IntegerToBinary(15, 6) + ".{154}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"MessageType1_1:uint:6",
				"SlotOffset1_1:uint:12",
				"Spare_1:uint:2",
				"MessageType1_2:uint:6",
				"SlotOffset1_2:uint:12",
				"Spare_2:uint:2",
				"DestinationMMSI2:uint:30",
				"MessageType2_1:uint:6",
				"SlotOffset2_1:uint:12",
				"Spare_3:uint:2"
			});

			// Type 15: Interrogation, 1 Station (ok)
			// DestinationID1 2 messages (ok)
			// DestinationID1 1 messages (ok)
			// Length: 110bit => 108bit
			patterns.Add("^" + IntegerToBinary(15, 6) + ".{102,154}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"MessageType1_1:uint:6",
				"SlotOffset1_1:uint:12",
				"Spare_1:uint:2",
				"MessageType1_2:uint:6",
				"SlotOffset1_2:uint:10",
				"Spare_2:uint:2"
			});

			// Type 15: Interrogation, pattern 1
			// DestinationID1 1 messages (ok)
			// Length: 88bit
			patterns.Add("^" + IntegerToBinary(15, 6) + ".{82,102}", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"DestinationMMSI1:uint:30",
				"MessageType1_1:uint:6",
				"SlotOffset1_1:uint:12"
			});


			// Type 16: Assignment Mode Command
			// With 2 Destinations
			// Length: 144bit
			patterns.Add("^" + IntegerToBinary(16, 6) + ".{142}", new string[] {
				 "MessageType:uint:6",
				 "RepeatIndicator:uint:2",
				 "SourceMMSI:uint:30",
				 "Spare:uint:2",
				 "DestinationMMSI1:uint:30",
				 "Offset1:uint:12",
				 "Increment1:uint:10",
				 "DestinationMMSI2:uint:30",
				 "Offset2:uint:12",
				 "Increment2:uint:10"
			 });

			// Type 16: Assignment Mode Command
			// With 1 Destinations
			// Length: 96bit
			patterns.Add("^" + IntegerToBinary(16, 6) + ".{90,142}", new string[] {
				 "MessageType:uint:6",
				 "RepeatIndicator:uint:2",
				 "SourceMMSI:uint:30",
				 "Spare:uint:2",
				 "DestinationMMSI1:uint:30",
				 "Offset1:uint:12",
				 "Increment1:uint:10",
				 "Reserved:uint:4"
			 });


			// Type 17: DGNSS Broadcast binary message
			patterns.Add("^" + IntegerToBinary(17, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"Lontitude:double:18:600",
				"Lattitude:double:17:600",
				"Spare2:uint:5",
				"Payload:data:0,736"
			});


			// Type 18: Class B Equipment Position Report (ok)
			patterns.Add("^" + IntegerToBinary(18, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"RegionalReserved:uint:8",
				"SpeedOverGround:double:10:10",
				"PositionAccuracy:uint:1",
				"Lontitude:double:28:600000",
				"Lattitude:double:27:600000",
				"CourseOverGround:double:12:10",
				"TrueHeading:uint:9",
				"TimeStamp:uint:6",
				"RegionalReserved2:uint:2",
				"CSUnit:uint:1",
				"DisplayFlag:uint:1",
				"DSCFlag:uint:1",
				"BandFlag:uint:1",
				"Message22Flag:uint:1",
				"Assigned:uint:1",
				"RAIMFlag:uint:1",
				"CommunicationStateSelectorFlag:uint:1",
				"SyncState:uint:2",   // CommunicationState (SOTDMA)
				"SlotTimeOut:uint:3",
				"SubMessage:uint:14"
			});


			// Type 19: Class B Equipment Position Report, Response to Interrogation (ok)
			patterns.Add("^" + IntegerToBinary(19, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"RegionalReserved:uint:8",
				"SpeedOverGround:double:10:10",
				"PositionAccuracy:uint:1",
				"Lontitude:double:28:600000",
				"Lattitude:double:27:600000",
				"CourseOverGround:double:12:10",
				"TrueHeading:uint:9",
				"TimeStamp:uint:6",
				"RegionalReserved2:uint:4",
				"Name:string:120",
				"ShipType:uint:8",
				"DimensionToBow:uint:9",
				"DimensionToStern:uint:9",
				"DimensionToPort:uint:6",
				"DimensionToStarboard:uint:6",
				"PositionFixType:uint:4",
				"RAIMFlag:uint:1",
				"DTE:uint:1",
				"AssignedModeFlag:uint:1",
				"Spare:uint:4"
			});


			// Type 20: Data Link Management Message (w/ 4 slot) (ok)
			// Length: 160bit
			patterns.Add("^" + IntegerToBinary(20, 6) + ".{154}.*", new string[] {
				 "MessageType:uint:6",
				 "RepeatIndicator:uint:2",
				 "SourceMMSI:uint:30",
				 "Spare:uint:2",
				 "OffsetNumber1:uint:12",
				 "ReservedSlots1:uint:4",
				 "Timeout1:uint:3",
				 "Increment1:uint:11",
				 "OffsetNumber2:uint:12", //90
				 "ReservedSlots2:uint:4",
				 "Timeout2:uint:3",
				 "Increment2:uint:11",
				 "OffsetNumber3:uint:12",
				 "ReservedSlots3:uint:4",
				 "Timeout3:uint:3",
				 "Increment3:uint:11",
				 "OffsetNumber4:uint:12",
				 "ReservedSlots4:uint:4",
				 "Timeout4:uint:3",
				 "Increment4:uint:11"
			});

			// Type 20: Data Link Management Message (w/ 3 slot) (ok)
			// Length: 130bit
			patterns.Add("^" + IntegerToBinary(20, 6) + ".{124,154}.*", new string[] {
				 "MessageType:uint:6",
				 "RepeatIndicator:uint:2",
				 "SourceMMSI:uint:30",
				 "Spare:uint:2",
				 "OffsetNumber1:uint:12",
				 "ReservedSlots1:uint:4",
				 "Timeout1:uint:3",
				 "Increment1:uint:11",
				 "OffsetNumber2:uint:12",
				 "ReservedSlots2:uint:4",
				 "Timeout2:uint:3",
				 "Increment2:uint:11",
				 "OffsetNumber3:uint:12",
				 "ReservedSlots3:uint:4",
				 "Timeout3:uint:3",
				 "Increment3:uint:11"
			});

			// Type 20: Data Link Management Message (w/ 2 slot) (ok)
			// Length: 100bit
			patterns.Add("^" + IntegerToBinary(20, 6) + ".{94,124}.*", new string[] {
				 "MessageType:uint:6",
				 "RepeatIndicator:uint:2",
				 "SourceMMSI:uint:30",
				 "Spare:uint:2",
				 "OffsetNumber1:uint:12",
				 "ReservedSlots1:uint:4",
				 "Timeout1:uint:3",
				 "Increment1:uint:11",
				 "OffsetNumber2:uint:12", //90
				 "ReservedSlots2:uint:4",
				 "Timeout2:uint:3",
				 "Increment2:uint:11"
			});

			// Type 20: Data Link Management Message (w/ 1 slot) (ok)
			// Length: 70bit
			patterns.Add(
				 "^" + IntegerToBinary(20, 6) + ".{64,94}.*", new string[] {
				 "MessageType:uint:6",
				 "RepeatIndicator:uint:2",
				 "SourceMMSI:uint:30",
				 "Spare:uint:2",
				 "OffsetNumber1:uint:12",
				 "ReservedSlots1:uint:4",
				 "Timeout1:uint:3",
				 "Increment1:uint:11"
			});


			// Type 21: Aid-to-Navigation Report
			patterns.Add("^" + IntegerToBinary(21, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"SourceMMSI:uint:30",
				"AidType:uint:5",
				"Name:string:120",
				"PositionAccuracy:uint:1",
				"Lontitude:double:28:600000",
				"Lattitude:double:27:600000",
				"DimensionToBow:uint:9",
				"DimensionToStern:uint:9",
				"DimensionToPort:uint:6",
				"DimensionToStarboard:uint:6",
				"PositionFixType:uint:4",
				"TimeStamp:uint:6",
				"OffPosition:uint:1",
				"RegionalReserved:uint:8",
				"RAIMFlag:uint:1",
				"VirtualAidFlag:uint:1",
				"AssignedModeFlag:uint:1",
				"Spare:uint:1",
				"NameExtension:string:0,88"
			});


			// Type 22: Channel Management
			patterns.Add("^" + IntegerToBinary(22, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:iint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"ChannelA:uint:12",
				"ChannelB:uint:12",
				"TransmitReceiveMode:uint:4",
				"Power:uint:1",
				"NELontitude:double:18:600",
				"NELattitude:double:17:600",
				"SWLontitude:double:18:600",
				"SWLattitude:double:17:600",
				"DestinationMMSI1:uint:30",
				"DestinationMMSI2:uint:30",
				"Addressed:uint:1",
				"ChannelABand:uint:1",
				"ChannelBBand:uint:1",
				"ZoneSite:unit:3",
				"Spare:uint:23"		  
			});


			// Type 23: Group Assignment Command
			patterns.Add("^" + IntegerToBinary(23, 6) + ".*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:iint:2",
				"SourceMMSI:uint:30",
				"Spare:uint:2",
				"NELontitude:double:18:600",
				"NELattitude:double:17:600",
				"SWLontitude:double:18:600",
				"SWLattitude:double:17:600",
				"StationType:uint:4",
				"ShipType:uint:8",
				"Spare1:uint:22",  
				"TransmitReceiveMode:uint:2",
				"ReportInterval:uint:4",
				"QuietTime:unit:4",
				"Spare2:uint:6"		  
			});


			// Type 24: Class B Static Data Report, Part A (ok)
			patterns.Add("^" + IntegerToBinary(24, 6) + ".{2}.{30}00.*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"PartNumber:uint:2",
				"VesselName:string:120"
			});

			// Type 24: Class B Static Data Report, Part B (ok)
			patterns.Add("^" + IntegerToBinary(24, 6) + ".{2}.{30}01.*", new string[] {
				"MessageType:uint:6",
				"RepeatIndicator:uint:2",
				"MMSI:uint:30",
				"PartNumber:uint:2",
				"ShipType:uint:8",
				"VendorID:string:42",
				"CallSign:string:42",
				"DimensionToBow:uint:9",
				"DimensionToStern:uint:9",
				"DimensionToPort:uint:6",
				"DimensionToStarboard:uint:6",
				"Spare:uint:6"
			});


			// Type 25: Single Slot Binary Message


			// Type 26: Multiple Slot Binary Message


			// Type 27: Long Range AIS Broadcast message


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
				aisdata = aisdata + new string('0', fillbits);
			}

			if (msg_number == msg_part)
			{
				if (msg_part != 1)
				{
					//Debug.WriteLine(string.Format("Slot:[{0}] Part:{1}/{2}", msg_slot, msg_number, msg_part));
					if (buffer.ContainsKey(msg_slot))
					{
						//Debug.WriteLine(string.Format("Length:{0}", aisdata.Length));
						if (buffer[msg_slot].Length % 6 == 0)
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
					//uint MessageType = Convert.ToUInt32(aisdata.Substring(0, 6), 2);
					//Debug.WriteLine(string.Format("MessageType::{0:00}", MessageType));
					//if (MessageType == 5)
					//    Debugger.Break();

					foreach (string key in patterns.Keys)
					{
						Regex regex = new Regex(key);
						if (regex.IsMatch(aisdata))
						{
							return DecodeAISData(aisdata, (string[])patterns[key]);
						}
					}
					Debug.WriteLine(aisdata);
					Debug.WriteLine(string.Format("Length::{0}", aisdata.Length));
					//Debugger.Break();
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
						if (buffer[msg_slot].Length % 6 == 0)
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
			//uint MessageType = Convert.ToUInt32(AISData.Substring(0, 6), 2);
			//Debug.WriteLine(string.Format("MessageType:{0:00}", MessageType));
			//if (MessageType == 5)
			//    Debugger.Break();

			Hashtable returnData = new Hashtable();
			int currentPosition = 0;

			for (int i = 0; i < attributes.Length; i++)
			{
				string[] data = attributes[i].Split(':');
				string fieldName = data[0];
				string fieldType = data[1];
				int fieldMinLength = 0;
				int fieldMaxLength = 0;
				int divisor = 1;

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
					case "uint":
						returnData.Add(fieldName, Convert.ToUInt32(AISData.Substring(currentPosition, fieldMinLength), 2));
						currentPosition += fieldMinLength;
						break;

					case "int":
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
				c -= 48;
				if (c > 40)
					c -= 8;

				//if (c >= 48 && c < 88)
				//    c -= 48;
				//else if (c >= 96 && c < 120)
				//    c -= 56;
				//else
				//    c = 0;

				string decodedChar = Convert.ToString(c, 2);
				for (int j = decodedChar.Length; j < 6; j++)
				{
					decodedChar = "0" + decodedChar;
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
			double returnValue = (double)Convert.ToInt64(encodedData, 2);

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
