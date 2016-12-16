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
			//Act
			//Assert
		}

		TEST_METHOD(bounceBack_Test) {
			//Arrange
			//Act
			//Assert
		}

		TEST_METHOD(flowThrough_Test) {
			//Arrange
			//Act
			//Assert
		}

		TEST_METHOD(setSinglePixel_Test) {
			//Arrange
			//Act
			//Assert
		}


	};
}