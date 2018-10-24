using System;
using System.Collections.Generic;
using System.Linq;
using TestSystemLibrary;
using static TestSystemLibrary.TestSystemBuilder;

namespace SpaceTestSystem
{
    class Program
    {
        static TestSystem testSystem;
        static void Main(string[] args)
        {
            // Система тестирования TestSystemLibrary проработана для обработки параметров типа string, int и decimal.
            // Для добавления новых типов параметров необходимо добавить конверторы значений IParameter в заданный тип 
            // в ядре системы - TestSystem.cs
            //
            // В рамках проработанных типов данных система позволяет добавлять новые параметры, с описанием логики
            // их валидации, порядка вывода, добавления тестов на основе этих параметров с гибкой настройкой алгоритма
            // прохождения теста, задания критериев прохождения тестирования.
            //
            // В консольном приложении приведены настройки системы согласнно талице из тестового задания


            try
            {
                //Создаём тестовую систему с заданными настройками
                testSystem = BuildTestSystem(SetStartParameters(), SetTests(), SetSuccessCriteria());

                //Запускаем рабочий цикл
                do {
                    //Обрабатываем входные параметры
                    ProcessingInputParameters();

                    //Запускаем тесты
                    testSystem.StartTests();

                    //Выводим результат
                    ShowResult();
                    
                } while (checkOnTestRepeate());
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Установка входных параметров системы
        /// </summary>
        /// <returns></returns>
        private static List<IParameter> SetStartParameters()
        {
            List<IParameter> parameters = new List<IParameter>();
            parameters.Add(new Parameter<string>(name: "Имя", description: "Имя в строковом формате", order: 0, validationExpr: s => !string.IsNullOrEmpty(s), validationErrorText: "Непустая строка"));
            parameters.Add(new Parameter<int>("Вес", "Целое число, больше ноля", 1, s => s > 0, "Целое число, больше ноля"));
            parameters.Add(new Parameter<int>("Рост", "Целое число, больше ноля", 2, s => s > 0, "Целое число, больше ноля"));
            parameters.Add(new Parameter<int>("Возраст", "Целое число, больше ноля", 3, s => s > 0, "Целое число, больше ноля"));
            parameters.Add(new Parameter<decimal>("Зрение", "Дробное число, от 0 до 1", 4, s => s >= 0 && s <= 1, "Дробное число, от 0 до 1"));
            parameters.Add(new Parameter<string>("Болезни и вредные привычки", "Вводить списком через пробел", 5, s => true, ""));
            return parameters;
        }

        /// <summary>
        /// Описание тестов
        /// </summary>
        /// <returns></returns>
        private static List<Test> SetTests()
        {
            List<Test> tests = new List<Test>();
            Func<Grade> func = () =>
            {
                if (GetIntParamValue("Вес") >= 75 && GetIntParamValue("Вес") <= 90)
                    return Grade.Хорошо;

                else if ((GetIntParamValue("Вес") >= 70 && GetIntParamValue("Вес") < 75)
                        || (GetIntParamValue("Вес") > 90 && GetIntParamValue("Вес") <= 100))
                    return Grade.Удовлетворительно;

                else
                    return Grade.Неудовлетворительно;
            };
            tests.Add(new Test(name: "Вес", description: "", order: 0, testExpr: func));

            func = () =>
            {
                if (GetIntParamValue("Рост") >= 170 && GetIntParamValue("Рост") <= 185)
                    return Grade.Хорошо;

                else if ((GetIntParamValue("Рост") >= 160 && GetIntParamValue("Рост") < 170)
                        || (GetIntParamValue("Рост") > 185 && GetIntParamValue("Рост") <= 190))
                    return Grade.Удовлетворительно;

                else
                    return Grade.Неудовлетворительно;
            };
            tests.Add(new Test("Рост", "", 1, func));

            func = () =>
            {
                if (GetIntParamValue("Возраст") >= 25 && GetIntParamValue("Возраст") <= 35)
                    return Grade.Хорошо;

                else if ((GetIntParamValue("Возраст") >= 23 && GetIntParamValue("Возраст") < 25)
                        || (GetIntParamValue("Возраст") > 35 && GetIntParamValue("Возраст") <= 37))
                    return Grade.Удовлетворительно;

                else
                    return Grade.Неудовлетворительно;
            };
            tests.Add(new Test("Возраст", "", 2, func));

            func = () =>
            {
                if (GetDecimalParamValue("Зрение") == 1)
                    return Grade.Хорошо;

                else
                    return Grade.Неудовлетворительно;
            };
            tests.Add(new Test("Зрение", "", 3, func));

            func = () =>
            {
                if (!ContainsInStringArrayParam("Болезни и вредные привычки", "Курение"))
                    return Grade.Хорошо;

                else
                    return Grade.Неудовлетворительно;
            };
            tests.Add(new Test("Курение", "", 4, func));

            func = () =>
            {
                int count = CountInStringArrayParam("Болезни и вредные привычки", TherapistList);
                if (count < 3)
                    return Grade.Хорошо;

                else if (count == 3)
                    return Grade.Удовлетворительно;

                else
                    return Grade.Неудовлетворительно;
            };
            tests.Add(new Test("Терапевт", "", 5, func));

            func = () =>
            {
                int count = CountInStringArrayParam("Болезни и вредные привычки", PsychiatristList);
                if (count == 0)
                    return Grade.Хорошо;

                else if (count == 1)
                    return Grade.Удовлетворительно;

                else
                    return Grade.Неудовлетворительно;
            };
            tests.Add(new Test("Психиатр", "", 6, func));

            func = () =>
            {
                if (ContainsInStringArrayParam("Болезни и вредные привычки", "Курение")
                    && (ContainsInStringArrayParam("Болезни и вредные привычки", "Простуда")
                    || ContainsInStringArrayParam("Болезни и вредные привычки", "Вирусы"))
                    && (GetIntParamValue("Вес") > 120 && GetIntParamValue("Вес") < 60))
                    return Grade.Неудовлетворительно;

                else if ((ContainsInStringArrayParam("Болезни и вредные привычки", "Простуда")
                        || ContainsInStringArrayParam("Болезни и вредные привычки", "Вирусы"))
                        && GetIntParamValue("Вес") > 110)
                    return Grade.Удовлетворительно;

                else
                    return Grade.Хорошо;
            };
            tests.Add(new Test("Вес и вредные привычки", "", 7, func));

            func = () =>
            {
                if (GetStringParamValue("Имя").StartsWith("П", StringComparison.InvariantCultureIgnoreCase))
                    return Grade.Хорошо;

                else if (GetIntParamValue("Возраст") > 68)
                    return Grade.Удовлетворительно;

                else
                    return Grade.Неудовлетворительно;
            };
            tests.Add(new Test("Странный", "", 8, func));

            func = () =>
            {
                if ((GetIntParamValue("Рост") % 3) == 0 && ContainsInStringArrayParam("Болезни и вредные привычки", "Насморк"))
                    return Grade.Неудовлетворительно;

                else if ((GetIntParamValue("Рост") % 2) == 0)
                    return Grade.Хорошо;

                else
                    return Grade.Удовлетворительно;
            };
            tests.Add(new Test("Математический", "", 9, func));

            return tests;
        }

        /// <summary>
        /// Установка критерия успешного прохождения тестов
        /// </summary>
        /// <returns></returns>
        private static Func<Dictionary<Grade, int>, bool> SetSuccessCriteria()
        {
            return s => s[Grade.Удовлетворительно] < 3 && s[Grade.Неудовлетворительно] == 0;
        }

        /// <summary>
        /// Обработка входных параметров
        /// </summary>
        static void ProcessingInputParameters()
        {
            //Проходим по списку параметров
            foreach (IParameter param in testSystem.GetParameters().OrderBy(s => s.Order))
            {
                Console.Write(string.Format("{0}({1}): ", param.Name, param.Description));
                string incomingValue = Console.ReadLine();
                //Если входная строка не прошла валидацию, повторяем ввод
                while (!testSystem.SetAndCheckEnterParam(param, incomingValue))
                {
                    ShowError(param.ValidationErrorText);
                    incomingValue = Console.ReadLine();
                }               
            }
        }

        /// <summary>
        /// Вывод сообщения об ошибке в консоли
        /// </summary>
        /// <param name="text">Текст ошибки</param>
        static void ShowError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Вывод результат теста
        /// </summary>
        private static void ShowResult()
        {
            bool isSuccess;
            string Result = testSystem.GetResult(out isSuccess);


            if (isSuccess)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Result);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Проверка на продолжение тестирования
        /// </summary>
        /// <returns></returns>
        private static bool checkOnTestRepeate()
        {
            Console.WriteLine();
            Console.Write("Желаете проверить других кандидатов? (д/н, y/n) ");
            string answer = Console.ReadLine();

            while (!new[] { "д", "да", "н", "нет", "y", "yes", "n", "no" }.Contains(answer))
            {
                ShowError("Ответ не понял, повторите.");
                answer = Console.ReadLine();
            }

            if (new[] { "д", "да", "y", "yes" }.Contains(answer))
            {
                testSystem.ResetParamsValue();
                Console.Clear();
                return true;
            }

            return false;
        }
    }
}