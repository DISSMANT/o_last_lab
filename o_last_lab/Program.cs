using System;
using Google.OrTools.LinearSolver;

class Program
{
    static void Main()
    {
        SolveCuttingStockProblem();
        Console.WriteLine("------------------------------------------");
        SolveProductionOptimizationProblem();
    }

    static void SolveCuttingStockProblem()
    {
        // Создаем решатель для задачи 1
        Solver solver = Solver.CreateSolver("SCIP");
        if (solver == null)
        {
            Console.WriteLine("Не удалось создать решатель SCIP.");
            return;
        }

        // Переменные (количество прутьев для каждого паттерна)
        // Используем MakeNumVar с нижней границей 0.0 и верхней без ограничений
        Variable x1 = solver.MakeNumVar(0.0, double.PositiveInfinity, "x1");
        Variable x2 = solver.MakeNumVar(0.0, double.PositiveInfinity, "x2");
        Variable x3 = solver.MakeNumVar(0.0, double.PositiveInfinity, "x3");
        Variable x4 = solver.MakeNumVar(0.0, double.PositiveInfinity, "x4");
        Variable x5 = solver.MakeNumVar(0.0, double.PositiveInfinity, "x5");
        Variable x6 = solver.MakeNumVar(0.0, double.PositiveInfinity, "x6");

        // Требуемые количества заготовок
        int required45 = 40;
        int required35 = 30;
        int required50 = 20;

        // Ограничения по количеству заготовок
        // 45 см: 2*x1 + x4 + x6 ≥ 40
        solver.Add(2 * x1 + 0 * x2 + 0 * x3 + 1 * x4 + 0 * x5 + 1 * x6 >= required45);

        // 35 см: 3*x2 + x5 + x6 ≥ 30
        solver.Add(0 * x1 + 3 * x2 + 0 * x3 + 0 * x4 + 1 * x5 + 1 * x6 >= required35);

        // 50 см: 2*x3 + x4 + x5 ≥ 20
        solver.Add(0 * x1 + 0 * x2 + 2 * x3 + 1 * x4 + 1 * x5 + 0 * x6 >= required50);

        // Целевая функция: минимизация отходов
        // Отходы:
        // Паттерн 1: 20 см * x1
        // Паттерн 2: 5 см * x2
        // Паттерн 3: 10 см * x3
        // Паттерн 4: 15 см * x4
        // Паттерн 5: 25 см * x5
        // Паттерн 6: 30 см * x6
        Objective objective = solver.Objective();
        objective.SetCoefficient(x1, 20);
        objective.SetCoefficient(x2, 5);
        objective.SetCoefficient(x3, 10);
        objective.SetCoefficient(x4, 15);
        objective.SetCoefficient(x5, 25);
        objective.SetCoefficient(x6, 30);
        objective.SetMinimization();

        // Решаем задачу
        Solver.ResultStatus resultStatus = solver.Solve();

        Console.WriteLine("Задача 1: Оптимизация резки заготовок");
        if (resultStatus == Solver.ResultStatus.OPTIMAL)
        {
            Console.WriteLine("Найдено оптимальное решение:");
            Console.WriteLine($"x1 = {x1.SolutionValue()}");
            Console.WriteLine($"x2 = {x2.SolutionValue()}");
            Console.WriteLine($"x3 = {x3.SolutionValue()}");
            Console.WriteLine($"x4 = {x4.SolutionValue()}");
            Console.WriteLine($"x5 = {x5.SolutionValue()}");
            Console.WriteLine($"x6 = {x6.SolutionValue()}");
            Console.WriteLine($"Минимальные отходы = {objective.Value()}");
        }
        else
        {
            Console.WriteLine("Оптимальное решение не найдено.");
        }
    }

    static void SolveProductionOptimizationProblem()
    {
        // Создаем решатель для задачи 2
        Solver solver = Solver.CreateSolver("SCIP");
        if (solver == null)
        {
            Console.WriteLine("Не удалось создать решатель SCIP.");
            return;
        }

        // Переменные: время работы оборудования I
        Variable t_i1 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t_i1");
        Variable t_i2 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t_i2");
        Variable t_i3 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t_i3");
        Variable t_i4 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t_i4");

        // Переменные: время работы оборудования II
        Variable t_ii1 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t_ii1");
        Variable t_ii2 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t_ii2");
        Variable t_ii3 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t_ii3");
        Variable t_ii4 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t_ii4");

        // Требуемый объем продукции
        int reqP1 = 200;
        int reqP2 = 150;
        int reqP3 = 240;
        int reqP4 = 180;

        // Производительность (шт/час)
        int p_i_p1 = 10, p_i_p2 = 8, p_i_p3 = 12, p_i_p4 = 9;   // Оборудование I
        int p_ii_p1 = 9, p_ii_p2 = 7, p_ii_p3 = 11, p_ii_p4 = 10; // Оборудование II

        // Ограничения по производству
        solver.Add(p_i_p1 * t_i1 + p_ii_p1 * t_ii1 >= reqP1);
        solver.Add(p_i_p2 * t_i2 + p_ii_p2 * t_ii2 >= reqP2);
        solver.Add(p_i_p3 * t_i3 + p_ii_p3 * t_ii3 >= reqP3);
        solver.Add(p_i_p4 * t_i4 + p_ii_p4 * t_ii4 >= reqP4);

        // Ограничения по времени оборудования
        solver.Add(t_i1 + t_i2 + t_i3 + t_i4 <= 30); // Оборудование I
        solver.Add(t_ii1 + t_ii2 + t_ii3 + t_ii4 <= 40); // Оборудование II

        // Затраты (долл. за час производства конкретного вида продукции на конкретном оборудовании)
        double cost_i_p1 = 5, cost_i_p2 = 6, cost_i_p3 = 5, cost_i_p4 = 7;
        double cost_ii_p1 = 4, cost_ii_p2 = 5, cost_ii_p3 = 6, cost_ii_p4 = 6;

        Objective objective = solver.Objective();
        objective.SetCoefficient(t_i1, cost_i_p1);
        objective.SetCoefficient(t_i2, cost_i_p2);
        objective.SetCoefficient(t_i3, cost_i_p3);
        objective.SetCoefficient(t_i4, cost_i_p4);

        objective.SetCoefficient(t_ii1, cost_ii_p1);
        objective.SetCoefficient(t_ii2, cost_ii_p2);
        objective.SetCoefficient(t_ii3, cost_ii_p3);
        objective.SetCoefficient(t_ii4, cost_ii_p4);

        objective.SetMinimization();

        Solver.ResultStatus resultStatus = solver.Solve();

        Console.WriteLine("Задача 2: Оптимизация производственного времени и затрат");
        if (resultStatus == Solver.ResultStatus.OPTIMAL)
        {
            Console.WriteLine("Найдено оптимальное решение:");
            Console.WriteLine($"t_i1 = {t_i1.SolutionValue()}, t_i2 = {t_i2.SolutionValue()}, t_i3 = {t_i3.SolutionValue()}, t_i4 = {t_i4.SolutionValue()}");
            Console.WriteLine($"t_ii1 = {t_ii1.SolutionValue()}, t_ii2 = {t_ii2.SolutionValue()}, t_ii3 = {t_ii3.SolutionValue()}, t_ii4 = {t_ii4.SolutionValue()}");
            Console.WriteLine($"Минимальные затраты = {objective.Value()}");
        }
        else
        {
            Console.WriteLine("Оптимальное решение не найдено.");
        }
    }
}
