import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { getAdmin } from './auth.js'

export default function RequireLogin() {
  const location = useLocation()

  if (!getAdmin()) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  return <Outlet />
}
