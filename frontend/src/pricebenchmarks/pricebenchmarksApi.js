
// Calls to the Price Benchmarks Web API.
// The API runs on http://localhost:5024.
const API =
  (import.meta.env.VITE_API_URL ?? 'http://localhost:5024') +
  '/api/pricebenchmarks'

// Reads the API response and handles errors.
async function handle(response) {
  if (response.status === 204) return null

  const data = await response.json().catch(() => null)

  if (!response.ok) {
    throw new Error(
      data?.message ?? 'Something went wrong, please try again.'
    )
  }

  return data
}

// Get the available Danish cities.
export const getPriceBenchmarkCities = () =>
  fetch(`${API}/cities`).then(handle)

// Get all price benchmarks.
export const getAllPriceBenchmarks = () =>
  fetch(API).then(handle)

// Find one benchmark by ID.
export const findPriceBenchmark = (id) =>
  fetch(`${API}/${id}`).then(handle)

// Filter benchmarks by city and active status.
export function filterPriceBenchmarks(filters) {
  const params = new URLSearchParams()

  for (const [key, value] of Object.entries(filters)) {
    if (value !== '' && value !== null && value !== undefined) {
      params.append(key, value)
    }
  }

  return fetch(`${API}?${params}`).then(handle)
}

// Send Add and Update requests.
const send = (url, method, benchmark) =>
  fetch(url, {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(benchmark),
  }).then(handle)

// Add a benchmark.
export const addPriceBenchmark = (benchmark) =>
  send(API, 'POST', benchmark)

// Update an existing benchmark.
export const updatePriceBenchmark = (id, benchmark) =>
  send(`${API}/${id}`, 'PUT', benchmark)

// Delete a benchmark.
export const deletePriceBenchmark = (id) =>
  fetch(`${API}/${id}`, { method: 'DELETE' }).then(handle)
