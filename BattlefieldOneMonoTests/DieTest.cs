using BattlefieldOneXNATests;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class DieTest
	{
		[Fact]
		public void TestDieRoll()
		{
			var fakeDieRoller = new FakeDieRoller();
			fakeDieRoller.ClearDieRoll();
			fakeDieRoller.SetDieRoll = 5;
			fakeDieRoller.SetDieRoll = 2;
			fakeDieRoller.SetDieRoll = 7;

			int result = fakeDieRoller.DieRoll();
			Assert.Equal(5, result);

			result = fakeDieRoller.DieRoll();
			Assert.Equal(2, result);

			result = fakeDieRoller.DieRoll();
			Assert.Equal(7, result);
		}

		[Fact]
		public void TestDieReset()
		{
			var fakeDieRoller = new FakeDieRoller();
			fakeDieRoller.ClearDieRoll();
			fakeDieRoller.SetDieRoll = 5;
			fakeDieRoller.SetDieRoll = 3;
			fakeDieRoller.SetDieRoll = 6;

			fakeDieRoller.DieRoll();
			fakeDieRoller.DieRoll();

			fakeDieRoller.ClearDieRoll();
			fakeDieRoller.SetDieRoll = 3;

			int result = fakeDieRoller.DieRoll();

			Assert.Equal(3, result);
		}

		[Fact]
		public void TestDieWrapAround()
		{
			var fakeDieRoller = new FakeDieRoller();
			fakeDieRoller.ClearDieRoll();
			fakeDieRoller.SetDieRoll = 6;
			fakeDieRoller.SetDieRoll = 2;

			var result = fakeDieRoller.DieRoll();
			Assert.Equal(6, result);

			result = fakeDieRoller.DieRoll();
			Assert.Equal(2, result);

			result = fakeDieRoller.DieRoll();
			Assert.Equal(6, result);
		}
	}
}
