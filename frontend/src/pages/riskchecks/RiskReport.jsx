import { useEffect, useState } from 'react'
import { Link, useLocation, useParams } from 'react-router-dom'
import { formatDate, getPublishedTips, getRiskCheck, runPriceCheck, splitFlags, toInput, updateRiskCheck } from '../../api/riskChecks.js'
import PriceCheckBox from './PriceCheckBox.jsx'
import './riskchecks.css'

// What the levels mean, shown to the user
const LEVEL_TEXT = {
  Low: 'Few or no warning signs were found. Still follow the safety steps below.',
  Medium: 'Some warning signs were found. Check the listing carefully before you pay anything.',
  High: 'Several strong warning signs were found. Do not pay anything until you have seen the flat in person and checked the landlord.',
}

// Used only if the Safety tips API (tblSafetyTip) cannot be reached
const NEXT_STEPS = [
  'View the flat in person, or by a live video call, before paying anything.',
  'Never pay a deposit or rent by MoneyGram, Western Union or gift cards.',
  'Check that the landlord owns or manages the flat.',
  'Make sure you can register your address (CPR) at the flat.',
]

// RISK REPORT (RSC-34). Opening an unread report marks it as read (UserNotified = true).
export default function RiskReport() {
  const { id } = useParams()
  const location = useLocation()
  const [check, setCheck] = useState(null)
  const [error, setError] = useState('')
  const [price, setPrice] = useState(null)
  const [tips, setTips] = useState([])

  useEffect(() => {
    let cancelled = false
    async function load() {
      try {
        const c = await getRiskCheck(id)
        if (!c.userNotified) {
          await updateRiskCheck(id, { ...toInput(c), userNotified: true })
          c.userNotified = true
        }
        if (!cancelled) setCheck(c)
        // Price check (RSC-31) and safety tips (Ambika) – the report still works if either fails
        runPriceCheck(c.listingId).then((p) => { if (!cancelled) setPrice(p) }).catch(() => {})
        getPublishedTips().then((t) => { if (!cancelled) setTips(t.slice(0, 5)) }).catch(() => {})
      } catch (e) {
        if (!cancelled) setError(e.message)
      }
    }
    load()
    return () => { cancelled = true }
  }, [id])

  if (error) return <div className="message error">{error} <Link to="/riskchecks">Back to the list</Link></div>
  if (!check) return <p>Loading…</p>

  const flags = splitFlags(check.flags)

  return (
    <>
      {location.state?.message && <div className="message success">{location.state.message}</div>}
      <h1>Risk report</h1>

      <div className="card report-top">
        <div className="score">{check.riskScore}<small> / 100</small></div>
        <div>
          <span className={`level ${check.riskLevel}`}>{check.riskLevel} risk</span>
          <p style={{ margin: '0.5rem 0 0' }}>
            <strong>{check.listingAddress ?? `Listing ${check.listingId}`}</strong><br />
            Checked {formatDate(check.checkedDate)}
          </p>
        </div>
      </div>

      <div className="report-grid">
        <div className="card">
          <h2 style={{ marginTop: 0 }}>What we found</h2>
          <p>{LEVEL_TEXT[check.riskLevel]}</p>
          <ul className="flags">
            {flags.map((f) => <li key={f}>{f}</li>)}
          </ul>
          {check.reviewNote && <p className="hint">Admin note: {check.reviewNote}</p>}
          {price && <PriceCheckBox result={price} />}
          <p className="hint">This score is guidance based on the listing's details. It does not prove that anyone is a scammer.</p>
        </div>

        <div className="card">
          <h2 style={{ marginTop: 0 }}>Safety steps</h2>
          {tips.length > 0 ? (
            <ul className="tips-list">
              {tips.map((t) => <li key={t.tipId}><strong>{t.title}</strong> – {t.description}</li>)}
            </ul>
          ) : (
            <ul>
              {NEXT_STEPS.map((s) => <li key={s}>{s}</li>)}
            </ul>
          )}
        </div>
      </div>

      <div className="report-actions">
        <Link className="btn secondary" to="/riskchecks">Back to list</Link>
        <Link className="btn secondary" to={`/riskchecks/${id}/edit`}>Edit</Link>
        <Link className="btn danger" to={`/riskchecks/${id}/delete`}>Delete</Link>
      </div>
    </>
  )
}
