// Calls to the Listings Web API (owner: Aman Paudel).
// The API runs on http://localhost:5024 when started with: dotnet run --project src/RSC.Api
const API = (import.meta.env.VITE_API_URL ?? 'http://localhost:5024') + '/api/listings'

// Reads the answer from the API. If the API sent an error, throw its message
// so the page can show it to the user.
async function handle(response) {
  if (response.status === 204) return null
  const data = await response.json().catch(() => null)
  if (!response.ok) throw new Error(data?.message ?? 'Something went wrong, please try again.')
  return data
}

export const getCities = () => fetch(`${API}/cities`).then(handle)

export const getAllListings = () => fetch(API).then(handle)

export const findListing = (id) => fetch(`${API}/${id}`).then(handle)

// filters: { area, minRent, maxRent, furnished } - empty values are left out
export function filterListings(filters) {
  const params = new URLSearchParams()
  for (const [key, value] of Object.entries(filters)) {
    if (value !== '' && value !== null && value !== undefined) params.append(key, value)
  }
  return fetch(`${API}?${params}`).then(handle)
}

const send = (url, method, listing) =>
  fetch(url, {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(listing),
  }).then(handle)

export const addListing = (listing) => send(API, 'POST', listing)

export const updateListing = (id, listing) => send(`${API}/${id}`, 'PUT', listing)

export const deleteListing = (id) => fetch(`${API}/${id}`, { method: 'DELETE' }).then(handle)
