import { useEffect, useState } from 'react';
import { TaskForm } from './TaskForm';
import { getTasks, createTask, updateTask, deleteTask } from '../api/tasks.api';
import type { TaskDto, ApiError } from '../api/types';

export const TaskList = () => {
  const [tasks, setTasks] = useState<TaskDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editingTaskId, setEditingTaskId] = useState<number | null>(null);

  useEffect(() => {
    loadTasks();
  }, []);

  const loadTasks = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await getTasks();
      setTasks(data);
    } catch (err) {
      const apiErr = err as ApiError;
      setError(apiErr.error || 'Failed to load tasks');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateTask = async (data: { title: string }) => {
    const newTask = await createTask(data);
    setTasks([...tasks, newTask]);
  };

  const handleToggleComplete = async (task: TaskDto) => {
    try {
      const updatedTask = await updateTask(task.id, {
        isComplete: !task.isComplete,
      });
      setTasks(tasks.map((t) => (t.id === task.id ? updatedTask : t)));
    } catch (err) {
      const apiErr = err as ApiError;
      setError(apiErr.error || 'Failed to update task');
    }
  };

  const handleDeleteTask = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this task?')) {
      return;
    }

    try {
      await deleteTask(id);
      setTasks(tasks.filter((t) => t.id !== id));
    } catch (err) {
      const apiErr = err as ApiError;
      setError(apiErr.error || 'Failed to delete task');
    }
  };

  const handleEditTask = async (data: { title: string }) => {
    if (editingTaskId === null) return;
    
    try {
      const updatedTask = await updateTask(editingTaskId, data);
      setTasks(tasks.map((t) => (t.id === editingTaskId ? updatedTask : t)));
      setEditingTaskId(null);
    } catch (err) {
      const apiErr = err as ApiError;
      setError(apiErr.error || 'Failed to update task');
      throw err;
    }
  };

  if (isLoading) {
    return (
      <div className="loading-container">
        <div className="loading-spinner">Loading tasks...</div>
      </div>
    );
  }

  return (
    <div className="task-list-container">
      <header className="task-header">
        <h1>My Tasks</h1>
      </header>

      {error && (
        <div className="error-alert" role="alert">
          {error}
          <button onClick={() => setError(null)} className="close-btn">
            ×
          </button>
        </div>
      )}

      <div className="add-task-section">
        <h2>Add New Task</h2>
        <TaskForm onSubmit={handleCreateTask} />
      </div>

      <div className="tasks-section">
        <h2>Tasks ({tasks.length})</h2>
        
        {tasks.length === 0 ? (
          <div className="empty-state">
            <p>No tasks yet. Add your first task above!</p>
          </div>
        ) : (
          <ul className="task-list">
            {tasks.map((task) => (
              <li
                key={task.id}
                className={`task-item ${task.isComplete ? 'completed' : ''}`}
              >
                {editingTaskId === task.id ? (
                  <TaskForm
                    onSubmit={handleEditTask}
                    onCancel={() => setEditingTaskId(null)}
                    initialValue={task.title}
                    submitLabel="Save"
                  />
                ) : (
                  <>
                    <div className="task-content">
                      <input
                        type="checkbox"
                        checked={task.isComplete}
                        onChange={() => handleToggleComplete(task)}
                        aria-label={`Mark "${task.title}" as ${task.isComplete ? 'incomplete' : 'complete'}`}
                      />
                      <span className="task-title">{task.title}</span>
                      <span className="task-date">
                        {new Date(task.createdAt).toLocaleDateString()}
                      </span>
                    </div>
                    <div className="task-actions">
                      <button
                        onClick={() => setEditingTaskId(task.id)}
                        className="btn-edit"
                        aria-label={`Edit "${task.title}"`}
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteTask(task.id)}
                        className="btn-delete"
                        aria-label={`Delete "${task.title}"`}
                      >
                        Delete
                      </button>
                    </div>
                  </>
                )}
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
};
