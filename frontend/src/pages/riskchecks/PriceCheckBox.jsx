// Shows the result of the price check (RSC-31): listing rent per m² vs the city's price benchmark.
// Used on the Add/Edit form and on the Risk report.
export default function PriceCheckBox({ result }) {
  const level = result.suggestedScore >= 70 ? 'High' : result.suggestedScore >= 40 ? 'Medium' : 'Low'
  const hasBenchmark = result.benchmarkRentPerM2 !== null && result.benchmarkRentPerM2 !== undefined
  const kr = (n) => Number(n).toLocaleString('da-DK', { maximumFractionDigits: 0 })

  return (
    <div className="price-box">
      <h3>Price check – {result.city}</h3>
      {hasBenchmark ? (
        <table>
          <tbody>
            <tr><th>This listing</th><td>{kr(result.listingRentPerM2)} kr/m²</td></tr>
            <tr><th>Usual rent in {result.city}</th><td>{kr(result.benchmarkRentPerM2)} kr/m²</td></tr>
            <tr>
              <th>Difference</th>
              <td>
                {result.percentBelow >= 0
                  ? `${Math.round(result.percentBelow)}% below the benchmark`
                  : `${Math.round(-result.percentBelow)}% above the benchmark`}
              </td>
            </tr>
            <tr><th>Suggested score</th><td>{result.suggestedScore} <span className={`level ${level}`}>{level}</span></td></tr>
          </tbody>
        </table>
      ) : (
        <p>{result.flag}</p>
      )}
      {result.benchmarkSource && <p className="hint">Benchmark source: {result.benchmarkSource}</p>}
    </div>
  )
}
