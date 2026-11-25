const KEY = 'rscAdmin'

export function getAdmin() {
  try {
    const saved = sessionStorage.getItem(KEY)
    return saved ? JSON.parse(saved) : null
  } catch {
    return null
  }
}

export async function login(username, password) {
  let res
  try {
    res = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
    })
  } catch {
    throw new Error('Could not reach the server. Is the API running?')
  }

  if (res.status === 401) throw new Error('Wrong username or password.')
  if (!res.ok) throw new Error(`Login failed (${res.status})`)

  const admin = await res.json()
  try {
    sessionStorage.setItem(KEY, JSON.stringify(admin))
  } catch {
    // storage blocked: the admin stays signed in only until the page reloads
  }
  return admin
}

export function logout() {
  try {
    sessionStorage.removeItem(KEY)
  } catch {
    // nothing to remove
  }
}
