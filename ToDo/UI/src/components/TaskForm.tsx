import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useState } from 'react';
import type { ApiError } from '../api/types';

const taskSchema = z.object({
  title: z
    .string()
    .min(1, 'Task title is required')
    .max(200, 'Task title must be less than 200 characters'),
});

type TaskFormData = z.infer<typeof taskSchema>;

interface TaskFormProps {
  onSubmit: (data: TaskFormData) => Promise<void>;
  onCancel?: () => void;
  initialValue?: string;
  submitLabel?: string;
}

export const TaskForm = ({ 
  onSubmit, 
  onCancel, 
  initialValue = '', 
  submitLabel = 'Add Task' 
}: TaskFormProps) => {
  const [apiError, setApiError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<TaskFormData>({
    resolver: zodResolver(taskSchema),
    defaultValues: {
      title: initialValue,
    },
  });

  const handleFormSubmit = async (data: TaskFormData) => {
    setApiError(null);
    setIsLoading(true);

    try {
      await onSubmit(data);
      reset();
    } catch (error) {
      const apiErr = error as ApiError;
      setApiError(apiErr.error || 'Failed to save task');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="task-form">
      {apiError && (
        <div className="error-alert" role="alert">
          {apiError}
        </div>
      )}

      <form onSubmit={handleSubmit(handleFormSubmit)}>
        <div className="form-group-inline">
          <input
            type="text"
            placeholder="Enter task title..."
            {...register('title')}
            disabled={isLoading}
            aria-invalid={errors.title ? 'true' : 'false'}
            className={errors.title ? 'input-error' : ''}
          />
          <button type="submit" disabled={isLoading}>
            {isLoading ? 'Saving...' : submitLabel}
          </button>
          {onCancel && (
            <button type="button" onClick={onCancel} disabled={isLoading}>
              Cancel
            </button>
          )}
        </div>
        {errors.title && (
          <span className="error-message" role="alert">
            {errors.title.message}
          </span>
        )}
      </form>
    </div>
  );
};
