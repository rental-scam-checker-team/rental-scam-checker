import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { addRiskCheck, getRiskCheck, toInput, updateRiskCheck } from '../../api/riskChecks.js'
import './riskchecks.css'

const today = () => new Date().toISOString().slice(0, 10)

// ADD and EDIT a risk check (RSC-25, RSC-26). The API runs RiskCheck.Valid() and returns any errors.
export default function RiskCheckForm() {
  const { id } = useParams()
  const editing = Boolean(id)
  const navigate = useNavigate()
  const [form, setForm] = useState({
    listingId: '', riskScore: '', flags: '', reviewNote: '', checkedDate: today(), userNotified: false,
  })
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    if (!editing) return
    getRiskCheck(id).then((c) => setForm(toInput(c))).catch((e) => setError(e.message))
  }, [editing, id])

  function change(e) {
    const { name, value, type, checked } = e.target
    setForm({ ...form, [name]: type === 'checkbox' ? checked : value })
  }

  async function save(e) {
    e.preventDefault()
    setSaving(true)
    setError('')
    const data = {
      ...form,
      listingId: Number(form.listingId),
      riskScore: Number(form.riskScore),
      reviewNote: form.reviewNote.trim() === '' ? null : form.reviewNote,
    }
    try {
      if (editing) {
        await updateRiskCheck(id, data)
        navigate(`/riskchecks/${id}`, { state: { message: 'Risk check saved.' } })
      } else {
        const created = await addRiskCheck(data)
        navigate(`/riskchecks/${created.riskCheckId}`, { state: { message: 'Risk check added.' } })
      }
    } catch (e2) {
      setError(e2.message)
      setSaving(false)
    }
  }

  return (
    <>
      <h1>{editing ? `Edit risk check ${id}` : 'Add risk check'}</h1>
      {error && <div className="message error">{error}</div>}

      <form className="card rc-form" onSubmit={save} noValidate>
        <div>
          <label htmlFor="listingId">Listing ID</label>
          <input id="listingId" name="listingId" type="number" min="1" value={form.listingId} onChange={change} />
          <span className="hint">The listing that was checked.</span>
        </div>
        <div>
          <label htmlFor="riskScore">Risk score (0–100)</label>
          <input id="riskScore" name="riskScore" type="number" min="0" max="100" value={form.riskScore} onChange={change} />
        </div>
        <div className="full">
          <label htmlFor="flags">Flags</label>
          <textarea id="flags" name="flags" rows="3" maxLength="500" value={form.flags} onChange={change} />
          <span className="hint">Reasons found, separated by semicolons. 1–500 characters ({form.flags.length}/500).</span>
        </div>
        <div className="full">
          <label htmlFor="reviewNote">Review note (optional)</label>
          <textarea id="reviewNote" name="reviewNote" rows="2" maxLength="250" value={form.reviewNote} onChange={change} />
          <span className="hint">Up to 250 characters ({form.reviewNote.length}/250).</span>
        </div>
        <div>
          <label htmlFor="checkedDate">Checked date</label>
          <input id="checkedDate" name="checkedDate" type="date" max={today()} value={form.checkedDate} onChange={change} />
        </div>
        <div className="check">
          <input id="userNotified" name="userNotified" type="checkbox" checked={form.userNotified} onChange={change} />
          <label htmlFor="userNotified">User has read the report</label>
        </div>
        <div className="buttons">
          <button className="btn" type="submit" disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
          <Link className="btn secondary" to={editing ? `/riskchecks/${id}` : '/riskchecks'}>Cancel</Link>
        </div>
      </form>
    </>
  )
}
