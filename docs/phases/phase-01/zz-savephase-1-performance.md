# Phase 1 Performance Baseline

**Framework**: .NET Framework 4.8  
**Captured**: 2025-12-09  
**Environment**: Local Development (IIS Express)  
**Browser**: Chrome 131.x (DevTools Network Tab)  
**Cache**: Disabled (Disable cache checkbox in DevTools)  
**Test runs**: 5 per page  

## Pages Measured

- `/` - Homepage (album grid)
- `/Store/Details/1` - Album detail page
- `/Checkout/AddressAndPayment` - Checkout form (authenticated user required)

## Measurement Procedure

1. Open Chrome DevTools → Network tab
2. Check "Disable cache"
3. Hard refresh page (Ctrl+Shift+R)
4. Wait for "Load" event (blue line in timeline)
5. Record: Load time, DOMContentLoaded, request count, transfer size
6. Repeat 5 times per page

## Data Files

- `phase1-performance.csv` - Raw measurements (15 runs)
- `phase1-performance-summary.csv` - Calculated statistics

## Acceptance Criteria

**Phase 2 (Azure Deployment):**  
Target: Within ±20% of Phase 1 baseline (cloud network overhead expected)

**Phase 4 (.NET 9 Migration):**  
Target: Within ±10% of Phase 3 baseline (platform should not regress)

## Notes

- Outliers removed from final dataset (homepage run #3 @ 178ms, checkout run #5 @ 189ms)
- LocalDB on same machine (no network latency)
- First-time visitor simulation (cache disabled)