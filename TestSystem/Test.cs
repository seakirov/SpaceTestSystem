using System;
using static TestSystemLibrary.TestSystemBuilder;

namespace TestSystemLibrary
{
    public class Test
    {
        /// <summary>
        /// Название теста
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Описание теста
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Порядок прохождения теста
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Описание логики теста
        /// </summary>
        public Func<Grade> TestExpr { get; private set; }

        /// <summary>
        /// Результат теста
        /// </summary>
        public Grade Result { get; private set; }

        /// <summary>
        /// Проверка теста
        /// </summary>
        /// <returns></returns>
        public Grade Check()
        {
            Result = TestExpr();
            return Result;
        }

        public Test(string name, string description, int order, Func<Grade> testExpr)
        {
            Name = name;
            Description = description;
            Order = order;
            TestExpr = testExpr;
            Result = Grade.Нет;
        }

        public override string ToString()
        {
            if (Result != Grade.Нет)
                return string.Format(" * Тест '{0}' ({1})", Name, Result);
            else return string.Format("Тест {0} не проходили", Name);
        }
    }
}
