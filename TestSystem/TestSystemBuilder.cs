using System;
using System.Collections.Generic;
using System.Linq;

namespace TestSystemLibrary
{
    public static class TestSystemBuilder
    {
        static TestSystem testSystem;

        /// <summary>
        /// Список для проверки терапевтом
        /// </summary>
        public static string[] TherapistList { get; set; } =
        {
            "Насморк",
            "Бронхит",
            "Вирусы",
            "Аллергия",
            "Ангина",
            "Бессонница"
        };

        /// <summary>
        /// Список для проверки психиатром
        /// </summary>
        public static string[] PsychiatristList { get; set; } =
        {
            "Алкоголизм",
            "Бессонница",
            "Наркомания",
            "Травмы"
        };

        /// <summary>
        /// Оценка теста
        /// </summary>
        public enum Grade
        {
            Нет = -1,
            Неудовлетворительно = 2,
            Удовлетворительно,
            Хорошо
        }

        /// <summary>
        /// Билдер тестовой системы с предустановленными настройками
        /// </summary>
        /// <param name="parameters">Список входных параметров</param>
        /// <returns>Тестовая система</returns>
        public static TestSystem BuildTestSystem(List<IParameter> parameters, List<Test> tests, Func<Dictionary<Grade, int>, bool> successCriteria)
        {
            List<IParameter> _parameters = null;
            List<Test> _tests = null;
            Func<Dictionary<Grade, int>, bool> _successCriteria = null;

            //Если список параметров пустой, берём параметры по умолчанию
            if (parameters == null || parameters.Count == 0)
            {
                _parameters = SetDefaultParams();
            }
            else
            {
                _parameters = parameters;
            }

            if (tests == null || tests.Count == 0)
            {
                _tests = SetDefaultTests();
            }
            else
            {
                _tests = tests;
            }

            if (successCriteria == null)
            {
                _successCriteria = SetDefaultCriteria();
            }
            else
            {
                _successCriteria = successCriteria;
            }

            testSystem = new TestSystem(_parameters, _tests, _successCriteria);

            //Создаём тестовую систему
            return testSystem;
        }

        /// <summary>
        /// Билдер тестовой системы с дефолтными настройками
        /// </summary>
        /// <returns>Тестовая система</returns>
        public static TestSystem BuildTestSystem()
        {
            return BuildTestSystem(SetDefaultParams(), SetDefaultTests(), SetDefaultCriteria());
        }

        /// <summary>
        /// Дефолтные параметры
        /// </summary>
        /// <returns>список параметров</returns>
        private static List<IParameter> SetDefaultParams()
        {
            List<IParameter> parameters = new List<IParameter>();
            parameters.Add(new Parameter<string>(name: "Имя", description: "Имя в строковом формате", order: 0, validationExpr: s => !string.IsNullOrEmpty(s), validationErrorText: "Непустая строка"));
            parameters.Add(new Parameter<int>("Вес", "Целое число, больше ноля", 1, s => s > 0, "Целое число, больше ноля"));
            return parameters;
        }

        /// <summary>
        /// Дефолтные тесты
        /// </summary>
        /// <returns>Список тестов</returns>
        private static List<Test> SetDefaultTests()
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

            return tests;
        }

        /// <summary>
        /// Дефолтные критерии
        /// </summary>
        /// <returns>Функтор с критерием</returns>
        private static Func<Dictionary<Grade, int>, bool> SetDefaultCriteria()
        {
            return s => s[Grade.Удовлетворительно] == 0 && s[Grade.Неудовлетворительно] == 0;
        }

        #region Вспомогательные методы для построения тестов (для уменьшения размера тестовых выражений)

        /// <summary>
        /// Получение значения параметра по его имени
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <returns>Значение параметра</returns>
        public static string GetStringParamValue(string name)
        {
            return testSystem.GetStringParameterValueByName(name);
        }

        /// <summary>
        /// Получение значения параметра по его имени
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <returns>Значение параметра</returns>
        public static int GetIntParamValue(string name)
        {
            return testSystem.GetIntParameterValueByName(name);
        }

        /// <summary>
        /// Получение значения параметра по его имени
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <returns>Значение параметра</returns>
        public static decimal GetDecimalParamValue(string name)
        {
            return testSystem.GetDecimalParameterValueByName(name);
        }

        /// <summary>
        /// Проверка на вхождение строки в строковый параметр со списком значений
        /// </summary>
        /// <param name="paramName">Строковый параметр со списком значений</param>
        /// <param name="value">Проверяемое значение</param>
        /// <returns>Результат проверки</returns>
        public static bool ContainsInStringArrayParam(string paramName, string value)
        {
            return GetStringParamValue(paramName).Split(' ').Contains(value, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Подсчёт количества совпадений массива значений со списком значений строкового параметра
        /// </summary>
        /// <param name="paramName">Строковый параметр со списком значений</param>
        /// <param name="values">массив значений</param>
        /// <returns>Количество совпадений</returns>
        public static int CountInStringArrayParam(string paramName, string[] values)
        {
            return GetStringParamValue(paramName).Split(' ').Intersect(values, StringComparer.InvariantCultureIgnoreCase).Count();
        }

        #endregion
    }
}
