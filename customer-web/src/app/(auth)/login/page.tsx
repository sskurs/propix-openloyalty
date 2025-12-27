"use client"

import * as React from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"

export default function LoginPage() {
    const [email, setEmail] = React.useState("")
    const [isLoading, setIsLoading] = React.useState(false)
    const [error, setError] = React.useState("")
    const router = useRouter()

    async function onSubmit(event: React.SyntheticEvent) {
        event.preventDefault()
        setIsLoading(true)
        setError("")

        try {
            // Simulate API call to check if member exists using the new Customer API
            const res = await fetch(`/api/customer/profile?email=${email}`)

            if (res.ok) {
                const member = await res.json()
                localStorage.setItem("memberId", member.id)
                localStorage.setItem("memberData", JSON.stringify(member))
                router.push("/dashboard")
            } else {
                setError("Member not found or connection error.")
            }
        } catch (err) {
            setError("Failed to connect to the server.")
        } finally {
            setIsLoading(false)
        }
    }

    return (
        <>
            <div className="grid gap-2 text-center">
                <h1 className="text-4xl font-extrabold tracking-tight lg:text-5xl">Welcome Back</h1>
                <p className="text-muted-foreground text-lg">
                    Enter your email below to access your rewards
                </p>
            </div>
            <form onSubmit={onSubmit}>
                <div className="grid gap-4">
                    <div className="grid gap-2">
                        <Label htmlFor="email">Email</Label>
                        <Input
                            id="email"
                            type="email"
                            placeholder="m@example.com"
                            required
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            disabled={isLoading}
                        />
                    </div>
                    {error && <p className="text-sm text-red-500">{error}</p>}
                    <Button type="submit" className="w-full" disabled={isLoading}>
                        {isLoading ? "Checking..." : "Login"}
                    </Button>
                </div>
            </form>

            <div className="relative my-4">
                <div className="absolute inset-0 flex items-center">
                    <span className="w-full border-t" />
                </div>
                <div className="relative flex justify-center text-xs uppercase">
                    <span className="bg-background px-2 text-muted-foreground">
                        Or
                    </span>
                </div>
            </div>

            <div className="grid gap-2 text-center text-sm">
                Don&apos;t have an account?{" "}
                <Link href="/register" className="underline">
                    Sign up
                </Link>
            </div>
        </>
    )
}
