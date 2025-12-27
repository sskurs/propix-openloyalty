"use client"

import { useEffect, useState } from "react"
import { useAuth } from "./use-auth"

export interface UsageHistory {
    id: string;
    campaignName: string;
    usageCount: number;
    lastUsedAt: string | null;
}

export interface RedemptionHistory {
    id: string;
    rewardName: string;
    pointsSpent: number | null;
    cashbackSpent: number | null;
    quantity: number;
    status: string;
    createdAt: string;
}

export function useMemberHistory() {
    const { member } = useAuth()
    const [usage, setUsage] = useState<UsageHistory[]>([])
    const [redemptions, setRedemptions] = useState<RedemptionHistory[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        if (!member?.id) return

        const fetchHistory = async () => {
            setLoading(true)
            try {
                const [usageRes, redemptionRes] = await Promise.all([
                    fetch(`/api/members/${member.id}/usage-history`),
                    fetch(`/api/members/${member.id}/redemption-history`)
                ])

                if (!usageRes.ok || !redemptionRes.ok) {
                    throw new Error("Failed to fetch history")
                }

                const usageData = await usageRes.json()
                const redemptionData = await redemptionRes.json()

                setUsage(usageData)
                setRedemptions(redemptionData)
            } catch (err) {
                console.error("Error fetching history:", err)
                setError(err instanceof Error ? err.message : "Unknown error")
            } finally {
                setLoading(false)
            }
        }

        fetchHistory()
    }, [member?.id])

    return { usage, redemptions, loading, error }
}
