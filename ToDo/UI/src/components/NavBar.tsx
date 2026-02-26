import { useAuth } from '../providers/AuthProvider';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../api/auth.api';

export const NavBar = () => {
  const { user, clearAuth } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await authApi.logout();
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      clearAuth();
      navigate('/login', { replace: true });
    }
  };

  return (
    <nav className="app-nav">
      <div className="nav-content">
        <h1 className="app-title">ToDo App</h1>
        {user && (
          <div className="nav-user-section">
            <div className="user-info">
              <span className="user-email">{user.email}</span>
              {user.role === 'Admin' && (
                <span className={`user-role role-${user.role.toLowerCase()}`}>
                  {user.role}
                </span>
              )}
            </div>
            <button onClick={handleLogout} className="btn-logout">
              Logout
            </button>
          </div>
        )}
      </div>
    </nav>
  );
};
