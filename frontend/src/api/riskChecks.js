// All calls to the Risk checks Web API (src/RSC.Api/Controllers/RiskChecksController.cs).
const BASE = '/api/riskchecks'

// Turns an API error response into one readable message.
async function readError(res) {
  try {
    const body = await res.json()
    if (body.error) return body.error
    if (body.errors) return Object.values(body.errors).flat().join(' ')
    if (body.title) return body.title
  } catch {
    // not JSON
  }
  return `Request failed (${res.status})`
}

async function send(url, options = {}) {
  const res = await fetch(url, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })
  if (!res.ok) throw new Error(await readError(res))
  if (res.status === 204) return null
  return res.json()
}

// LIST and FILTER: any filter left empty is ignored.
export function listRiskChecks(filters = {}) {
  const params = new URLSearchParams()
  for (const [key, value] of Object.entries(filters)) {
    if (value !== '' && value !== null && value !== undefined) params.set(key, value)
  }
  const query = params.toString()
  return send(query ? `${BASE}?${query}` : BASE)
}

// FIND
export function getRiskCheck(id) {
  return send(`${BASE}/${id}`)
}

// ADD
export function addRiskCheck(data) {
  return send(BASE, { method: 'POST', body: JSON.stringify(data) })
}

// EDIT
export function updateRiskCheck(id, data) {
  return send(`${BASE}/${id}`, { method: 'PUT', body: JSON.stringify(data) })
}

// DELETE
export function deleteRiskCheck(id) {
  return send(`${BASE}/${id}`, { method: 'DELETE' })
}

// PRICE CHECK (RSC-31): compares the listing's rent per m² with the city's price benchmark
export function runPriceCheck(listingId) {
  return send(`${BASE}/pricecheck/${listingId}`)
}

// Published safety tips (Ambika's API), most important first
export async function getPublishedTips() {
  const res = await fetch('/api/safetytips?isPublished=true')
  if (!res.ok) return []
  return res.json()
}

// Helpers shared by the pages
export function toInput(check) {
  return {
    listingId: check.listingId,
    riskScore: check.riskScore,
    flags: check.flags,
    reviewNote: check.reviewNote ?? '',
    checkedDate: check.checkedDate.slice(0, 10),
    userNotified: check.userNotified,
  }
}

export function splitFlags(flags) {
  return flags.split(';').map((f) => f.trim()).filter(Boolean)
}

export function formatDate(value) {
  return new Date(value).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })
}
