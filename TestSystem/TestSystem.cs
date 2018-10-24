using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static TestSystemLibrary.TestSystemBuilder;

namespace TestSystemLibrary
{
    public class TestSystem
    {
        private List<IParameter> _parameters;
        private List<Test> _tests;
        private Func<Dictionary<Grade, int>, bool> _successCriteria;
        private Dictionary<Grade, int> _testResult;

        private static TestSystem instance;
              
        private TestSystem() { }

        public static TestSystem getInstance()
        {
            if (instance == null)
                instance = BuildTestSystem();
            return instance;
        }

        /// <summary>
        /// Конструктор тестовой системы с настройками
        /// </summary>
        /// <param name="parameters">Список параметров</param>
        /// <param name="tests">Список тестов</param>
        /// <param name="successCriteria">Критерии прохождения</param>
        internal TestSystem(List<IParameter> parameters, List<Test> tests, Func<Dictionary<Grade, int>, bool> successCriteria)
        {
            _parameters = parameters;
            _tests = tests;
            _successCriteria = successCriteria;
        }

        /// <summary>
        /// Очистка значений параметров
        /// </summary>
        public void ResetParamsValue()
        {
            foreach (IParameter param in _parameters)
            {  
                param.ResetValue();             
            }
        }

        /// <summary>
        /// Получение списка параметров тестовой системы
        /// </summary>
        /// <returns>Список параметров</returns>
        public List<IParameter> GetParameters()
        {
            return _parameters;
        }

        /// <summary>
        /// Установка и валидация входных параметров
        /// </summary>
        /// <param name="param">Параметр</param>
        /// <param name="incomingValue">Значение</param>
        /// <returns>Признак успешной валидации</returns>
        public bool SetAndCheckEnterParam(IParameter param, string incomingValue)
        {
            MethodInfo setValue = param.GetType().GetMethod("SetValue");

            switch (param.Type.ToString())
            {
                case "System.String":
                    Parameter<string> stringParam = GetStringParameter(param);
                    if (stringParam.ValidateValue(incomingValue))
                    {
                        setValue.Invoke(param, new object[] { incomingValue });
                        return true;
                    }
                    break;               
                case "System.Int32":
                    Parameter<int> intParam = GetIntParameter(param);
                    int intValue;
                    if (int.TryParse(incomingValue, out intValue) && intParam.ValidateValue(intValue))
                    {
                        setValue.Invoke(param, new object[] { intValue });
                        return true;
                    }
                    break;
                case "System.Decimal":
                    Parameter<decimal> decimalParam = GetDecimalParameter(param);
                    decimal decimalValue;
                    while (decimal.TryParse(incomingValue, out decimalValue) && decimalParam.ValidateValue(decimalValue))
                    {
                        setValue.Invoke(param, new object[] { decimalValue });
                        return true;
                    }                   
                    break;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Запуск тестов, заполнение результатов тестирования
        /// </summary>
        public void StartTests()
        {
            _testResult = new Dictionary<Grade, int>();
            foreach(Grade grade in (Grade[])Enum.GetValues(typeof(Grade)))
            {
                _testResult.Add(grade, 0);
            }

            foreach(Test test in _tests.OrderBy(s => s.Order))
            {
                _testResult[test.Check()]++;
            }
        }

        /// <summary>
        /// Проверка и возврат результатов тестов
        /// </summary>
        /// <param name="isSuccess">Пройден ли тест</param>
        /// <returns>Результат в виде текста</returns>
        public string GetResult(out bool isSuccess)
        {
            string Result = string.Empty;
            
            if (_successCriteria(_testResult))
            {
                Result = Environment.NewLine + string.Format("Кандидат {0} подходит", GetStringParameterValueByName("Имя"));

                isSuccess = true;
            }
            else
            {
                Result = Environment.NewLine + string.Format("Кандидат {0} не прошел тестирование. Проблемы:", GetStringParameterValueByName("Имя"));
                foreach(Test test in _tests)
                {
                    if (test.Result != Grade.Хорошо)
                    {
                        Result += Environment.NewLine + test.ToString();
                    }
                }
                isSuccess = false;
            }

            return Result;
        }

        #region Получение обобщенных параметров

        /// <summary>
        /// Получение параметра типа string по его интерфейсу
        /// </summary>
        /// <param name="param">Интерфейс параметра</param>
        /// <returns>Параметр типа string</returns>
        Parameter<string> GetStringParameter(IParameter param)
        {
            MethodInfo getObject = param.GetType().GetMethod("GetObject");
            return (Parameter<string>)getObject.Invoke(param, null);
        }

        /// <summary>
        /// Получение значения параметра по его имени
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <returns>Значение</returns>
        internal string GetStringParameterValueByName(string name)
        {
            IParameter param = _parameters.SingleOrDefault(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (param == null)
            {
                throw new ArgumentNullException("Не найден параметр с именем " + name + ". Обратитесь к администратору системы.");
            }

            return GetStringParameter(param).Value;
        }

        /// <summary>
        /// Получение параметра типа int по его интерфейс
        /// </summary>
        /// <param name="param">Интерфейс параметра</param>
        /// <returns>Параметр типа int</returns>
        Parameter<int> GetIntParameter(IParameter param)
        {
            MethodInfo getObject = param.GetType().GetMethod("GetObject");
            return (Parameter<int>)getObject.Invoke(param, null);
        }

        /// <summary>
        /// Получение значения параметра по его имени
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <returns>Значение</returns>
        internal int GetIntParameterValueByName(string name)
        {
            IParameter param = _parameters.SingleOrDefault(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (param == null)
            {
                throw new ArgumentNullException("Не найден параметр с именем " + name + ". Обратитесь к администратору системы.");
            }

            return GetIntParameter(param).Value;
        }

        /// <summary>
        /// Получение параметра типа decimal по его интерфейс
        /// </summary>
        /// <param name="param">Интерфейс параметра</param>
        /// <returns>Параметр типа decimal</returns>
        Parameter<decimal> GetDecimalParameter(IParameter param)
        {
            MethodInfo getObject = param.GetType().GetMethod("GetObject");
            return (Parameter<decimal>)getObject.Invoke(param, null);
        }

        /// <summary>
        /// Получение значения параметра по его имени
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <returns>Значение</returns>
        internal decimal GetDecimalParameterValueByName(string name)
        {
            IParameter param = _parameters.SingleOrDefault(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (param == null)
            {
                throw new ArgumentNullException("Не найден параметр с именем " + name + ". Обратитесь к администратору системы.");
            }

            return GetDecimalParameter(param).Value;
        }
        #endregion
    }
}