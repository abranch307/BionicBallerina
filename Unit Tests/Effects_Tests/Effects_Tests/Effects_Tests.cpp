#include "stdafx.h"
#include "CppUnitTest.h"
#include "Effects_MockObject.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace Effects_Tests
{		
	TEST_CLASS(UnitTest1)
	{
	public:
		
		TEST_METHOD(allClear_test)
		{
			//Arrange
			bool bret;
			LightingSequence* seqs1 = (LightingSequence*)calloc(1, sizeof(LightingSequence));
			seqs1[0] = { ALLCLEAR, 3, "0,0,0,0,0 ", 200, 7000, 1, 2, 100, 0, 0 };
			//Act
			bret = Effects_MockObject::allClear_Test( seqs1, 0);
			//Assert

			Assert::AreEqual(bret, true);

			for (int i = 0; i < 3; i++) {
				Assert::AreEqual(0, Effects_MockObject::theStrip[i]);
			}
		}

		TEST_METHOD(loadColor_Test) {
			//Arrange
			bool bret;
			LightingSequence* seqs1 = (LightingSequence*)calloc(1, sizeof(LightingSequence));
			seqs1[0] = { LOADCOLOR, 3, "0,1,2,", 200, 7000, 1, 2, 100, 0, 0 };
			//Act
			bret = Effects_MockObject::loadColor_Test(seqs1, 0, 2);
			//Assert

			Assert::AreEqual(bret, true);

			for (int i = 0; i < 3; i++) {
				Assert::AreEqual(i, Effects_MockObject::theStrip[i]);
			}
		}

		TEST_METHOD(bounceBack_Test) {
			//Arrange
			bool bret;
			bool init = false;
			bool forward = true;
			__int16 shiftPixels, head, tail, bounces;
			shiftPixels = 1;
			head = 1;
			tail = 1;
			bounces = 0;
			LightingSequence* seqs1 = (LightingSequence*)calloc(1, sizeof(LightingSequence));
			seqs1[0] = { BOUNCEBACK, 3, "0,1,0", 200, 7000, 1, 2, 100, 0, 0 };
			//Act
			bret = Effects_MockObject::bounceBack_Test(seqs1, 0, &init, &forward, &tail, &head, &bounces, 0, 1, 1);
			//Assert

			Assert::AreEqual(bret, true);

			
			Assert::AreEqual(0, Effects_MockObject::theStrip[0]);
			Assert::AreEqual(0, Effects_MockObject::theStrip[1]);
			Assert::AreEqual(1, Effects_MockObject::theStrip[2]);

		}

		TEST_METHOD(flowThrough_Test) {
			//Arrange
			//Act
			//Assert
		}

		TEST_METHOD(setSinglePixelColor_Test) {
			//Arrange
			bool bret;
			LightingSequence* seqs1 = (LightingSequence*)calloc(1, sizeof(LightingSequence));
			seqs1[0] = { LOADCOLOR, 3, "0,1,2,", 200, 7000, 1, 2, 100, 0, 0 };
			//Act
			bret = Effects_MockObject::setSinglePixelColor_Test(seqs1, 0, 2);
			//Assert

			Assert::AreEqual(bret, true);

			Assert::AreEqual(2, Effects_MockObject::theStrip[0]);
			
		}


	};
}