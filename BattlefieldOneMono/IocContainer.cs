using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace BattlefieldOneMono
{
	public static class IocContainer
	{
		public static IContainer Container { get; set; }

		public static void Setup()
		{
			var builder = new ContainerBuilder();
			builder.Register(x => new DieRoller())
				.As<IDieRoller>()
				.PropertiesAutowired()
				.SingleInstance();

			builder.Register(x => new BattleCalculator(x.Resolve<IDieRoller>()))
				.As<IBattleCalculator>()
				.PropertiesAutowired()
				.SingleInstance();

			builder.Register(x => new UnitList(x.Resolve<IBattleCalculator>(), x.Resolve<ITerrainMap>()))
				.As<IUnitList>()
				.PropertiesAutowired()
				.SingleInstance();

			builder.Register(x => new TerrainMap(x.Resolve<IShortestPath>()))
				.As<ITerrainMap>()
				.PropertiesAutowired()
				.SingleInstance();

			builder.Register(x => new ShortestPath())
				.As<IShortestPath>()
				.PropertiesAutowired()
				.SingleInstance();

			builder.Register(x => new AlliedEnemyMatrix(true))
				.As<IAlliedEnemyMatrix>()
				.PropertiesAutowired()
				.SingleInstance();

			builder.Register(x => new VictoryCalculator(x.Resolve<ITerrainMap>(), x.Resolve<IUnitList>()))
				.As<IVictoryCalculator>()
				.PropertiesAutowired()
				.SingleInstance();

			builder.Register(x => new EnemyPlan(x.Resolve<ITerrainMap>(), x.Resolve<IUnitList>(), x.Resolve<IAlliedEnemyMatrix>()))
				.As<IEnemyPlan>()
				.PropertiesAutowired()
				.SingleInstance();

			builder.Register(x => new GameBoard(x.Resolve<ITerrainMap>(), x.Resolve<IUnitList>(), 
				x.Resolve<IShortestPath>(), x.Resolve<IAlliedEnemyMatrix>(), 
				x.Resolve<IVictoryCalculator>(),x.Resolve<IEnemyPlan>()))
				.As<IGameBoard>()
				.PropertiesAutowired()
				.SingleInstance();

			builder.Register(x => new Unit(x.Resolve<IShortestPath>()))
				.As<IUnit>();

			Container = builder.Build();
		}
	}
}
