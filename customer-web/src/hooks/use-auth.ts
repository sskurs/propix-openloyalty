"use client"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"

export interface Member {
    id: string;
    firstName?: string;
    lastName?: string;
    email?: string;
    status?: string;
    joinDate?: string;
    tierName?: string;
    wallets?: {
        id: string;
        type: string;
        balance: number;
    }[];
}

export function useAuth() {
    const [member, setMember] = useState<Member | null>(null)
    const [loading, setLoading] = useState(true)
    const router = useRouter()

    useEffect(() => {
        const stored = localStorage.getItem("memberData")

        setTimeout(() => {
            if (!stored) {
                router.push("/login")
                setLoading(false)
            } else {
                try {
                    const data = JSON.parse(stored);
                    setMember(data)
                    setLoading(false)
                } catch (e) {
                    console.error("Auth error", e);
                    localStorage.removeItem("memberData")
                    router.push("/login")
                    setLoading(false)
                }
            }
        }, 0);
    }, [router])

    const logout = () => {
        localStorage.removeItem("memberData")
        localStorage.removeItem("memberId")
        router.push("/login")
    }

    return { member, loading, logout }
}
