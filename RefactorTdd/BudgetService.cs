using System;
using System.Linq;

namespace RefactorTdd
{
	public class BudgetService
	{
		private readonly IBudgetRepo _repo;

		private static void Main()
		{
		}

		public BudgetService(IBudgetRepo repo)
		{
			_repo = repo;
		}

		public double TotalAmount(DateTime start, DateTime end)
		{
			if (!IsValidDateRange(start, end))
			{
				return 0;
			}

			var budgets = _repo.GetAll();

			if (IsSameMonth(start, end))
			{
				var budget = budgets.SingleOrDefault(x => x.YearMonth.Equals(start.ToString("yyyyMM")));
				if (budget == null)
				{
					return 0;
				}

				var budgetPerDay = AmountPerDayInMonth(start, budget);
				var intervalDays = DaysInterval(start, end);
				
				return budgetPerDay * intervalDays;
			}
			else
			{
				DateTime currentMonth = new DateTime(start.Year, start.Month, 1);
				double aggrAmount = 0;
				do
				{
					var budgetByMonth =
						budgets.SingleOrDefault(x => x.YearMonth.Equals(currentMonth.ToString("yyyyMM")));
					if (budgetByMonth != null)
					{
						if (IsFirstMonth(start, currentMonth))
						{
							var dailyAmount = AmountPerDayInMonth(budgetByMonth, start);
							var intervalDays = (DateTime.DaysInMonth(start.Year, start.Month) - start.Day + 1);
							
							aggrAmount += dailyAmount * intervalDays;
						}
						else if (IsLastMonth(end, currentMonth))
						{
							var dailyAmount = AmountPerDayInMonth(budgetByMonth, end);
							var intervalDays = end.Day;
							
							aggrAmount += dailyAmount * intervalDays;
						}
						else
						{
							var dailyAmount = AmountPerDayInMonth(budgetByMonth, currentMonth);
							var intervalDays = DateTime.DaysInMonth(currentMonth.Year,currentMonth.Month);
							
							aggrAmount += dailyAmount * intervalDays;
						}
					}

					currentMonth = currentMonth.AddMonths(1);
				} while (currentMonth <= end);

				return aggrAmount;
			}
		}

		private static int AmountPerDayInMonth(DateTime start, Budget budget)
		{
			return budget.Amount / DateTime.DaysInMonth(start.Year, start.Month);
		}

		private static bool IsLastMonth(DateTime end, DateTime tempDate)
		{
			return tempDate.ToString("yyyyMM") == end.ToString("yyyyMM");
		}

		private static bool IsFirstMonth(DateTime start, DateTime tempDate)
		{
			return tempDate.ToString("yyyyMM") == start.ToString("yyyyMM");
		}

		private static bool IsSameMonth(DateTime start, DateTime end)
		{
			return start.ToString("yyyyMM") == end.ToString("yyyyMM");
		}

		private static int AmountPerDayInMonth(Budget budgetByMonth, DateTime tempDate)
		{
			return budgetByMonth.Amount / DateTime.DaysInMonth(tempDate.Year, tempDate.Month);
		}

		private static bool IsValidDateRange(DateTime start, DateTime end)
		{
			return start <= end;
		}

		private static int DaysInterval(DateTime start, DateTime end)
		{
			return end.Subtract(start).Days + 1;
		}
	}
}