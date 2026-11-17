import { useState } from 'react'
import { addListing, updateListing } from './listingsApi'

// Add a listing (RSC-18) and Edit a listing (RSC-19).
// If `listing` is given, the form is in Edit mode and starts with its values.
export default function ListingForm({ listing, cities, onSaved, onCancel }) {
  const editing = Boolean(listing)
  // today's date as yyyy-mm-dd in local time (the posted date cannot be after this)
  const [today] = useState(() => new Date().toLocaleDateString('sv-SE'))

  const [form, setForm] = useState({
    address: listing?.address ?? '',
    area: listing?.area ?? '',
    monthlyRent: listing ? String(listing.monthlyRent) : '',
    sizeM2: listing ? String(listing.sizeM2) : '',
    postedDate: listing ? listing.postedDate.slice(0, 10) : today,
    furnished: listing?.furnished ?? false,
  })
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)

  const change = (field) => (e) => {
    const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value
    setForm({ ...form, [field]: value })
  }

  async function save(e) {
    e.preventDefault()
    setError('')
    setSaving(true)
    // Everything is sent as text. The API runs Listing.Valid() and sends back
    // one message per wrong field if something is not right.
    const data = { ...form, furnished: String(form.furnished) }
    try {
      const saved = editing ? await updateListing(listing.listingId, data) : await addListing(data)
      onSaved(saved, editing ? 'Listing updated.' : 'Listing saved.')
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="listings">
      <div className="ls-header">
        <h1>{editing ? `Edit listing ${listing.listingId}` : 'Add a listing'}</h1>
      </div>

      {error && <div className="message error">{error}</div>}

      <form className="card ls-form" onSubmit={save} noValidate>
        <div className="full">
          <label htmlFor="f-address">Address</label>
          <input id="f-address" value={form.address} onChange={change('address')} maxLength={100}
                 placeholder="Nørrebrogade 45, 2. th." />
          <span className="hint">Street address from the ad, up to 100 characters.</span>
        </div>

        <div>
          <label htmlFor="f-area">City</label>
          <select id="f-area" value={form.area} onChange={change('area')}>
            <option value="">Choose a city</option>
            {cities.map((c) => <option key={c} value={c}>{c}</option>)}
          </select>
          <span className="hint">Used to find the right price benchmark.</span>
        </div>

        <div>
          <label htmlFor="f-rent">Monthly rent (kr)</label>
          <input id="f-rent" type="number" step="0.01" min="0" value={form.monthlyRent} onChange={change('monthlyRent')} />
          <span className="hint">More than 0 and up to 50,000 kr.</span>
        </div>

        <div>
          <label htmlFor="f-size">Size (m²)</label>
          <input id="f-size" type="number" step="1" value={form.sizeM2} onChange={change('sizeM2')} />
          <span className="hint">A whole number from 5 to 500.</span>
        </div>

        <div>
          <label htmlFor="f-date">Date the ad was posted</label>
          <input id="f-date" type="date" max={today} value={form.postedDate} onChange={change('postedDate')} />
          <span className="hint">Cannot be in the future.</span>
        </div>

        <div className="check full">
          <input id="f-furnished" type="checkbox" checked={form.furnished} onChange={change('furnished')} />
          <label htmlFor="f-furnished" style={{ margin: 0 }}>Furnished</label>
        </div>

        <div className="buttons">
          <button className="btn" type="submit" disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
          <button className="btn secondary" type="button" onClick={onCancel}>Cancel</button>
        </div>
      </form>
    </div>
  )
}
