using System;

namespace TestSystemLibrary
{
    /// <summary>
    /// Обёртка над обобщёнными параметрами для единой коллекции и базовой работы с данными параметров
    /// </summary>
    public interface IParameter
    {
        string Name { get; }
        string Description { get; }
        int Order { get; }
        string ValidationErrorText { get; }
        Type Type { get; }
        void ResetValue();
    }
}