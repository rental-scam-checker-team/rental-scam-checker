import { useState } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'
import { getAdmin, login } from './auth.js'

export default function LoginPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const goTo = location.state?.from || '/riskchecks'

  if (getAdmin()) {
    return <Navigate to={goTo} replace />
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')

    if (!username.trim() || !password) {
      setError('Please enter your username and password.')
      return
    }

    setBusy(true)
    try {
      await login(username.trim(), password)
      navigate(goTo, { replace: true })
    } catch (err) {
      setError(err.message)
      setBusy(false)
    }
  }

  return (
    <div className="login-page">
      <form className="card login-card" onSubmit={handleSubmit}>
        <h1>Admin login</h1>
        <p className="hint">Sign in to manage listings, risk checks, price benchmarks and safety tips.</p>

        {error && <p className="message error" role="alert">{error}</p>}

        <label htmlFor="username">Username</label>
        <input
          id="username"
          autoComplete="username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />

        <label htmlFor="password">Password</label>
        <input
          id="password"
          type="password"
          autoComplete="current-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <button type="submit" className="btn" disabled={busy}>
          {busy ? 'Signing in…' : 'Log in'}
        </button>
      </form>
    </div>
  )
}
