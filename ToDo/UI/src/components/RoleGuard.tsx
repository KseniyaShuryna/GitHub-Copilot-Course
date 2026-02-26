import { useAuth } from '../providers/AuthProvider';

interface RoleGuardProps {
  children: React.ReactNode;
  allowedRoles: ('User' | 'Admin')[];
  fallback?: React.ReactNode;
}

/**
 * Component that conditionally renders children based on user role
 */
export const RoleGuard = ({ children, allowedRoles, fallback = null }: RoleGuardProps) => {
  const { user } = useAuth();

  if (!user || !allowedRoles.includes(user.role)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
};

/**
 * Hook to check if user has a specific role
 */
export const useRole = () => {
  const { user } = useAuth();

  const hasRole = (role: 'User' | 'Admin') => {
    return user?.role === role;
  };

  const isAdmin = () => user?.role === 'Admin';
  const isUser = () => user?.role === 'User';

  return {
    role: user?.role,
    hasRole,
    isAdmin,
    isUser,
  };
};
