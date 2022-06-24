using MGroup.MSolve.Discretization.Entities;
using MGroup.LinearAlgebra.Vectors;
using MGroup.Constitutive.Thermal;
using MGroup.NumericalAnalyzers;
using MGroup.NumericalAnalyzers.Dynamic;
using MGroup.Solvers.Direct;
using MGroup.Solvers.Iterative;
using MGroup.FEM.Thermal.Tests.ExampleModels;

namespace MGroup.FEM.Thermal.Tests.Integration
{
	public class ThermalBenchmarkProvatidis11_2Dynamic
	{
		public static void RunTest()
		{
			Model model = Provatidis_11_2_Example.CreateModel();
			IVectorView solution = SolveModel(model);


			for (int i=0; i<solution.Length; i++)
				Console.WriteLine(solution[i]);

			if(Provatidis_11_2_Example.CompareResults(solution))
            {
				Console.WriteLine("Correct Results");
            }
			else
            {
				Console.WriteLine("WANK!");
            }
		}

		private static IVectorView SolveModel(Model model)
		{
			var solverFactory = new SkylineSolver.Factory();
			var algebraicModel = solverFactory.BuildAlgebraicModel(model);
			var solver = solverFactory.BuildSolver(algebraicModel);
			var problem = new ProblemThermal(model, algebraicModel, solver);

			var linearAnalyzer = new LinearAnalyzer(algebraicModel, solver, problem);
			var dynamicAnalyzerBuilder = new BDFDynamicAnalyzer.Builder(model, algebraicModel, solver, problem, linearAnalyzer, timeStep: 0.5, totalTime: 1000, bdfOrder : 5);
 			var dynamicAnalyzer = dynamicAnalyzerBuilder.Build();

			dynamicAnalyzer.Initialize();
			dynamicAnalyzer.Solve();

			return solver.LinearSystem.Solution.SingleVector;
		}
	}
}

