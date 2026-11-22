import { useEffect, useState } from 'react'
import './SafetyTipsPage.css'

const API_URL = 'http://localhost:5024/api/safetytips'
const CATEGORIES = ['Payment', 'Contact', 'Price', 'Contract', 'Viewing']

function emptyForm() {
  return {
    title: '',
    description: '',
    category: 'Payment',
    priority: '3',
    isPublished: false,
    dateAdded: new Date().toISOString().slice(0, 10),
  }
}

function SafetyTipsPage() {
  const [tips, setTips] = useState([])
  const [category, setCategory] = useState('')
  const [published, setPublished] = useState('')
  const [findId, setFindId] = useState('')
  const [form, setForm] = useState(emptyForm())
  const [editingId, setEditingId] = useState(null)
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  async function loadTips() {
    const params = new URLSearchParams()
    if (category) params.append('category', category)
    if (published) params.append('isPublished', published)

    try {
      const response = await fetch(`${API_URL}?${params}`)
      setTips(await response.json())
      setError('')
    } catch {
      setError('Could not reach the API. Is it running?')
    }
  }

  useEffect(() => {
    loadTips()
  }, [category, published])

  async function findTip() {
    if (!findId) {
      loadTips()
      return
    }

    const response = await fetch(`${API_URL}/${findId}`)
    if (response.status === 404) {
      setTips([])
      setMessage(`No safety tip with id ${findId}.`)
      return
    }

    setTips([await response.json()])
    setMessage('')
  }

  function updateField(event) {
    const { name, value, type, checked } = event.target
    setForm({ ...form, [name]: type === 'checkbox' ? checked : value })
  }

  async function saveTip(event) {
    event.preventDefault()

    const response = await fetch(editingId ? `${API_URL}/${editingId}` : API_URL, {
      method: editingId ? 'PUT' : 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form),
    })

    if (response.status === 400) {
      setError(await response.text())
      return
    }

    setError('')
    setMessage(editingId ? 'Tip updated.' : 'Tip added.')
    setForm(emptyForm())
    setEditingId(null)
    loadTips()
  }

  function startEdit(tip) {
    setEditingId(tip.tipId)
    setForm({
      title: tip.title,
      description: tip.description,
      category: tip.category,
      priority: String(tip.priority),
      isPublished: tip.isPublished,
      dateAdded: tip.dateAdded.slice(0, 10),
    })
    setMessage('')
    setError('')
  }

  function cancelEdit() {
    setEditingId(null)
    setForm(emptyForm())
    setError('')
  }

  async function deleteTip(tip) {
    if (!window.confirm(`Delete "${tip.title}"?`)) return

    await fetch(`${API_URL}/${tip.tipId}`, { method: 'DELETE' })
    setMessage('Tip deleted.')
    loadTips()
  }

  return (
    <main className="tips-page">
      <h1>Safety tips</h1>

      {message && <p className="tips-message">{message}</p>}
      {error && <p className="tips-error">{error}</p>}

      <section className="tips-filters">
        <label>
          Category
          <select value={category} onChange={(e) => setCategory(e.target.value)}>
            <option value="">All</option>
            {CATEGORIES.map((c) => (
              <option key={c} value={c}>{c}</option>
            ))}
          </select>
        </label>

        <label>
          Published
          <select value={published} onChange={(e) => setPublished(e.target.value)}>
            <option value="">All</option>
            <option value="true">Published</option>
            <option value="false">Hidden</option>
          </select>
        </label>

        <label>
          Find by id
          <input type="number" value={findId} onChange={(e) => setFindId(e.target.value)} />
        </label>
        <button type="button" onClick={findTip}>Find</button>
      </section>

      <table className="tips-table">
        <thead>
          <tr>
            <th>Id</th>
            <th>Title</th>
            <th>Category</th>
            <th>Priority</th>
            <th>Published</th>
            <th>Date added</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {tips.map((tip) => (
            <tr key={tip.tipId}>
              <td>{tip.tipId}</td>
              <td>{tip.title}</td>
              <td>{tip.category}</td>
              <td>{tip.priority}</td>
              <td>{tip.isPublished ? 'Yes' : 'No'}</td>
              <td>{tip.dateAdded.slice(0, 10)}</td>
              <td>
                <button type="button" onClick={() => startEdit(tip)}>Edit</button>
                <button type="button" onClick={() => deleteTip(tip)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <form className="tips-form" onSubmit={saveTip}>
        <h2>{editingId ? `Edit tip ${editingId}` : 'Add a tip'}</h2>

        <label>
          Title
          <input name="title" value={form.title} onChange={updateField} />
        </label>

        <label>
          Description
          <textarea name="description" rows="3" value={form.description} onChange={updateField} />
        </label>

        <label>
          Category
          <select name="category" value={form.category} onChange={updateField}>
            {CATEGORIES.map((c) => (
              <option key={c} value={c}>{c}</option>
            ))}
          </select>
        </label>

        <label>
          Priority
          <select name="priority" value={form.priority} onChange={updateField}>
            {['1', '2', '3', '4', '5'].map((p) => (
              <option key={p} value={p}>{p}</option>
            ))}
          </select>
        </label>

        <label>
          Date added
          <input type="date" name="dateAdded" value={form.dateAdded} onChange={updateField} />
        </label>

        <label className="tips-checkbox">
          <input type="checkbox" name="isPublished" checked={form.isPublished} onChange={updateField} />
          Published (users can see it)
        </label>

        <div>
          <button type="submit">{editingId ? 'Save changes' : 'Add tip'}</button>
          {editingId && <button type="button" onClick={cancelEdit}>Cancel</button>}
        </div>
      </form>
    </main>
  )
}

export default SafetyTipsPage