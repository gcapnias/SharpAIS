using SharpAIS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System;

namespace SharpAISTest
{


	/// <summary>
	///This is a test class for AISParserTest and is intended
	///to contain all AISParserTest Unit Tests
	///</summary>
	[TestClass()]
	public class AISParserTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for ParseSentence
		///</summary>
		[TestMethod()]
		public void ParseSentenceTest()
		{
			string sentence = string.Empty; // TODO: Initialize to an appropriate value
			Hashtable expected = null; // TODO: Initialize to an appropriate value
			Hashtable actual;
			actual = AISParser.ParseSentence(sentence);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for IntegerToBinary
		///</summary>
		[TestMethod()]
		[DeploymentItem("SharpAIS.dll")]
		public void IntegerToBinaryTest()
		{
			Random rnd = new Random();

			int intergerValue = rnd.Next(64);
			int outputLength = 6;
			string expected = Convert.ToString(intergerValue, 2);
			string actual = AISParser_Accessor.IntegerToBinary(intergerValue, outputLength);
			Assert.AreEqual(true, actual.EndsWith(expected));
			Assert.AreEqual(6, actual.Length);
		}

		/// <summary>
		///A test for EncodedDataToBinary
		///</summary>
		[TestMethod()]
		[DeploymentItem("SharpAIS.dll")]
		public void EncodedDataToBinaryTest()
		{
			string encodedData = "012345";
			string expected = "000000000001000010000011000100000101";
			string actual = AISParser_Accessor.EncodedDataToBinary(encodedData);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for DecodeAISData
		///</summary>
		[TestMethod()]
		[DeploymentItem("SharpAIS.dll")]
		public void DecodeAISDataTest()
		{
			string AISData = "1CR7p0001S1bL>BEdk:S18Bh0P00";
			string[] attributes = new string[] {
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
				"SubΜessage:data:14"
			};
			Hashtable expected = new Hashtable();
			expected.Add("Spare", (int)0);
			expected.Add("UserID", (int)237107200);
			expected.Add("NavigationalStatus", (int)0);
			expected.Add("SpecialManeuvreIndicator", (int)0);
			expected.Add("SubΜessage", "00000000000000");
			expected.Add("CourseOverGround", (double)77.2M);
			expected.Add("RepeatIndicator", (int)1);
			expected.Add("MessageID", (int)1);
			expected.Add("Longitude", (double)23.2523883333333M);
			expected.Add("SyncState", (int)1);
			expected.Add("RAIMFlag", (int)0);
			expected.Add("TrueHeading", (int)265);
			expected.Add("PositionAccuracy", (int)0);
			expected.Add("Latitude", (double)37.9234833333333M);
			expected.Add("RateOfTurn", (int)0);
			expected.Add("TimeStamp", (int)24);
			expected.Add("SpeedOverGround", (int)99);
			expected.Add("SlotTimeOut", (int)0);
			Hashtable actual = AISParser_Accessor.DecodeAISData(AISParser_Accessor.EncodedDataToBinary(AISData), attributes);
			foreach (DictionaryEntry item in expected)
			{
				Assert.IsTrue(actual.Contains(item.Key));
				Assert.AreEqual(item.Value, actual[item.Key]);
			}
			Assert.AreEqual(expected.Count, actual.Count);
		}

		/// <summary>
		///A test for BinToString
		///</summary>
		[TestMethod()]
		[DeploymentItem("SharpAIS.dll")]
		public void BinToStringTest()
		{
			string encodedData = string.Empty; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = AISParser_Accessor.BinToString(encodedData);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for BinToSignedInteger
		///</summary>
		[TestMethod()]
		[DeploymentItem("SharpAIS.dll")]
		public void BinToSignedIntegerTest()
		{
			string encodedData = string.Empty; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = AISParser_Accessor.BinToSignedInteger(encodedData);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for BinToSignedDouble
		///</summary>
		[TestMethod()]
		[DeploymentItem("SharpAIS.dll")]
		public void BinToSignedDoubleTest()
		{
			string encodedData = string.Empty; // TODO: Initialize to an appropriate value
			double expected = 0F; // TODO: Initialize to an appropriate value
			double actual;
			actual = AISParser_Accessor.BinToSignedDouble(encodedData);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for AISParser Constructor
		///</summary>
		[TestMethod()]
		public void AISParserConstructorTest()
		{
			AISParser target = new AISParser();
			Assert.Inconclusive("TODO: Implement code to verify target");
		}


		/// <summary>
		/// Test decoding on type 5 message
		/// </summary>
		[TestMethod()]
		public void ParseMessageType5Text()
		{
			Parser p = new Parser();
			string data = "!AIVDM,2,1,7,B,53R7uN01QqPM=KWG?D0l58uDh40000000000001@5HD286vR07ThCU3l,0*76";
			Hashtable result = p.Parse(data);
			Assert.AreEqual(result, null);

			data = "!AIVDM,2,2,7,B,RCR@00000000000,2*23";
			result = p.Parse(data);
			Assert.AreNotEqual(result, null);
			string VesselName = result["VesselName"].ToString();
			Assert.AreEqual(VesselName, "MAROULA");

			data = "!AIVDM,2,1,1,A,5CUH?:02<fvq=`p:221@PDthtLu>122r222222153iHB45Ld0;42DPAE,0*1B";
			result = p.Parse(data);
			Assert.AreEqual(result, null);

			data = "!AIVDM,2,2,1,A,Dp8888888888880,2*11";
			result = p.Parse(data);
			Assert.AreNotEqual(result, null);
			VesselName = result["VesselName"].ToString();
			Assert.AreEqual(VesselName, "THEOLOGOS P .       ");

		}

		/// <summary>
		/// Test decoding on type 15 message
		/// </summary>
		[TestMethod()]
		public void ParseMessageType15Text()
		{
			Parser p = new Parser();
			string data = "!AIVDM,1,1,,A,?3T>HV1HMF;0D00,2*61";
			Hashtable result = p.Parse(data);
			Assert.AreNotEqual(result, null);
			Assert.AreEqual((uint)result["SourceMMSI"], (uint)239311000);
			Assert.AreEqual((uint)result["DestinationMMSI1"], (uint)371022000);

		}

	}
}
