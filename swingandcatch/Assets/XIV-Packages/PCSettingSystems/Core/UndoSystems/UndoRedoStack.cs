using System.Collections.Generic;

namespace XIV_Packages.PCSettingSystems.Core.UndoSystems
{
    public class UndoRedoStack<T> where T : ICommand
    {
        public int undoCount => undoStack.Count;
        public int redoCount => redoStack.Count;

        Stack<T> undoStack;
        Stack<T> redoStack;

        public UndoRedoStack()
        {
            undoStack = new();
            redoStack = new();
        }

        public UndoRedoStack(UndoRedoStack<T> undoRedoStack)
        {
            undoStack = new Stack<T>(undoRedoStack.undoStack);
            redoStack = new Stack<T>(undoRedoStack.redoStack);
        }

        public void Do(T command)
        {
            undoStack.Push(command);
            redoStack.Clear();
            command.Execute();
        }

        public T Undo()
        {
            var command = undoStack.Pop();
            redoStack.Push(command);
            command.Unexecute();
            return command;
        }

        public T Redo()
        {
            var command = redoStack.Pop();
            undoStack.Push(command);
            command.Execute();
            return command;
        }

        public T UndoPeek()
        {
            return undoStack.Peek();
        }

        public T RedoPeek()
        {
            return redoStack.Peek();
        }

        public void Clear()
        {
            undoStack.Clear();
            redoStack.Clear();
        }

        public IEnumerable<T> GetUndoOperations()
        {
            return undoStack;
        }

        public IEnumerable<T> GetRedoOperations()
        {
            return redoStack;
        }
    }
}