using System;

namespace TestSystemLibrary
{
    public class Parameter<T> : IParameter
    {
        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Описание параметра
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Порядок вывода параметров
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Значение параметра
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Выражение для ограничения области значений параметра
        /// </summary>
        Func<T, bool> ValidationExpr { get; set; }

        /// <summary>
        /// Текст в случае неудачи при валидации
        /// </summary>
        public string ValidationErrorText { get; private set; }

        /// <summary>
        /// Тип параметра
        /// </summary>
        public Type Type { get { return typeof(T); } }

        /// <summary>
        /// Получение объекта
        /// </summary>
        /// <returns></returns>
        public Parameter<T> GetObject()
        {
            return this;
        }

        /// <summary>
        /// Установка значения
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Сброс значения на дефолтное
        /// </summary>
        public void ResetValue()
        {
            Value = default(T);
        }

        /// <summary>
        /// Валидация значения
        /// </summary>
        /// <returns></returns>
        public bool ValidateValue()
        {
            return ValidationExpr(Value);
        }

        /// <summary>
        /// Валидация внешнего значения
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ValidateValue(T value)
        {
            return ValidationExpr(value);
        }

        public Parameter() { }
      
        ///
        public Parameter(string name, string description, int order, Func<T, bool> validationExpr, string validationErrorText)
        {         

            Name = name;
            Description = description;
            Order = order;
            ValidationExpr = validationExpr;
            ValidationErrorText = validationErrorText;
        }
    }
}
