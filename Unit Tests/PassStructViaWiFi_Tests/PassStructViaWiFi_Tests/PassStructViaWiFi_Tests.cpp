#include "stdafx.h"
#include "CppUnitTest.h"
#include "PSVW_MockObject.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace PassStructViaWiFi_Tests
{		
	TEST_CLASS(UnitTests)
	{
	public:
		/*
			This test uses a mock object to imitate the PassStructViaWiFi's begin method.  This mock object
			verifies passed ssid and passphrase match default ssid and passphrase.  The mock object also
			simulates the concatenation of ESP_ with ESP8266's chip id to form the hostname that user will
			eventually see
		*/
		TEST_METHOD(begin_TestSuccess)
		{
			//Arrange
			//Initialize variables
			PSVW_MockObject psvw;
			const char *ssid = "DOTSTARCOMPOSER", *passphrase = "dotstar1234", *espSuffix = "C102B";
			bool bret = false;

			//Act
			bret = psvw.begin_Test(ssid, passphrase, espSuffix);

			//Assert
			Assert::AreEqual(bret, true);
		}

		/*
			Similar to above begin_TestSuccess method, except this method expects a false return to called
			mock object method due to invalid SSID
		*/
		TEST_METHOD(begin_TestSSIDFailure)
		{
			//Arrange
			//Initialize variables
			PSVW_MockObject psvw;
			const char *ssid = "DOTST", *passphrase = "dotstar1234", *espSuffix = "C102B";
			bool bret = false;

			//Act
			bret = psvw.begin_Test(ssid, passphrase, espSuffix);

			//Assert
			Assert::AreEqual(bret, false);
		}

		/*
			Similar to above begin_TestSuccess method, except this method expects a false return to called
			mock object method due to invalid Passphrase
		*/
		TEST_METHOD(begin_TestPassphraseFailure)
		{
			//Arrange
			//Initialize variables
			PSVW_MockObject psvw;
			const char *ssid = "DOTSTARCOMPOSER", *passphrase = "dotstar12", *espSuffix = "C102B";
			bool bret = false;

			//Act
			bret = psvw.begin_Test(ssid, passphrase, espSuffix);

			//Assert
			Assert::AreEqual(bret, false);
		}

		/*
			Similar to above begin_TestSuccess method, except this method expects a false return to called
			mock object method due to invalid ESPSuffix
		*/
		TEST_METHOD(begin_TestESPSuffixFailure)
		{
			//Arrange
			PSVW_MockObject psvw;
			const char *ssid = "DOTST", *passphrase = "dotstar1234", *espSuffix = "C102";
			bool bret = false;

			//Act
			bret = psvw.begin_Test(ssid, passphrase, espSuffix);

			//Assert
			Assert::AreEqual(bret, false);
		}

		/*
			This test uses a mock object to imitate the PassStructViaWiFi's handleReady method.  This mock object
			verifies passed URI has valid Uri, Param, and Args.
		*/
		TEST_METHOD(HandleReady_Test)
		{
			//Arrange
			PSVW_MockObject psvw;
			bool bret = false;
			char Uri[] = "http://192.168.0.101/ready?READY=Y";

			//Act
			bret = psvw.handleReady(Uri);

			//Assert
			Assert::AreEqual(bret, true);
		}

		/*
			This test uses a mock object to imitate the PassStructViaWiFi's handleInitLEDSeqs method.  This mock object
			verifies passed URI has valid Uri, Param, and Args.
		*/
		TEST_METHOD(HandleInitLEDSeqs_Test)
		{
			//Arrange
			PSVW_MockObject psvw;
			bool bret = false;
			char Uri[] = "http://192.168.0.101/init_led_seq?INITLEDSEQ=0";

			//Act
			bret = psvw.handleInitLEDSeqs(Uri);

			//Assert
			Assert::AreEqual(bret, true);
		}
	};
}