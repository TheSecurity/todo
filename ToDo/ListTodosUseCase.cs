﻿namespace ToDo
{
    public class ListTodosUseCase : IListTodosUseCase
    {
        private readonly ITaskStorage _storage;

        public ListTodosUseCase(ITaskStorage storage)
        {
            _storage = storage; 
        }

        public TodoTask[] Execute() => _storage.RetrieveAll();
    }
}
