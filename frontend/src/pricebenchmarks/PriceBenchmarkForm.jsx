
import { useState } from 'react'
import { addPriceBenchmark, updatePriceBenchmark } from './pricebenchmarksApi'

// Add (RSC-36) and Edit a price benchmark.
export default function PriceBenchmarkForm({
  benchmark,
  cities,
  onSaved,
  onCancel,
}) {
  const editing = Boolean(benchmark)

  // Today's date in yyyy-mm-dd format.
  const [today] = useState(() => new Date().toLocaleDateString('sv-SE'))

  const [form, setForm] = useState({
    city: benchmark?.city ?? '',
    privateRentPerM2:
      benchmark?.privateRentPerM2 != null
        ? String(benchmark.privateRentPerM2)
        : '',
    socialRentPerM2:
      benchmark?.socialRentPerM2 != null
        ? String(benchmark.socialRentPerM2)
        : '',
    isActive: benchmark?.isActive ?? true,
    dataDate: benchmark?.dataDate
      ? benchmark.dataDate.slice(0, 10)
      : today,
    sourceReference: benchmark?.sourceReference ?? '',
  })

  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)

  const change = (field) => (e) => {
    const value =
      e.target.type === 'checkbox'
        ? e.target.checked
        : e.target.value

    setForm({ ...form, [field]: value })
  }

  async function save(e) {
    e.preventDefault()
    setError('')
    setSaving(true)

    try {
      const saved = editing
        ? await updatePriceBenchmark(benchmark.benchmarkId, form)
        : await addPriceBenchmark(form)

      onSaved(
        saved,
        editing
          ? 'Price benchmark updated.'
          : 'Price benchmark saved.'
      )
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  return (
    <form className="pricebenchmark-form" onSubmit={save}>
      <h2>
        {editing
          ? `Edit benchmark ${benchmark.benchmarkId}`
          : 'Add a price benchmark'}
      </h2>

      <label>
        City
        <select
          value={form.city}
          onChange={change('city')}
          required
        >
          <option value="">Choose a city</option>
          {cities.map((city) => (
            <option key={city} value={city}>
              {city}
            </option>
          ))}
        </select>
      </label>

      <label>
        Private rent per m² (DKK)
        <input
          type="number"
          step="0.01"
          min="0"
          value={form.privateRentPerM2}
          onChange={change('privateRentPerM2')}
          placeholder="Optional"
        />
      </label>

      <label>
        Social rent per m² (DKK)
        <input
          type="number"
          step="0.01"
          min="0"
          value={form.socialRentPerM2}
          onChange={change('socialRentPerM2')}
          placeholder="Optional"
        />
      </label>

      <label>
        Data date
        <input
          type="date"
          max={today}
          value={form.dataDate}
          onChange={change('dataDate')}
          required
        />
      </label>

      <label>
        Source reference
        <textarea
          value={form.sourceReference}
          onChange={change('sourceReference')}
          maxLength={200}
          rows={3}
          placeholder="Enter the data source..."
          required
        />
      </label>

      <label className="pricebenchmark-checkbox">
        <input
          type="checkbox"
          checked={form.isActive}
          onChange={change('isActive')}
        />
        Active benchmark
      </label>

      {error && <p className="error">{error}</p>}

      <div className="buttons">
        <button type="submit" disabled={saving}>
          {saving ? 'Saving...' : 'Save'}
        </button>

        <button type="button" onClick={onCancel}>
          Cancel
        </button>
      </div>
    </form>
  )
}
