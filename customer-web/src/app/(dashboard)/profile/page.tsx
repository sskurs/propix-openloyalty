"use client"

import { useAuth } from "@/hooks/use-auth"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

export default function ProfilePage() {
    const { member } = useAuth()

    return (
        <div className="space-y-6">
            <h2 className="text-3xl font-bold tracking-tight">Profile</h2>
            <Card>
                <CardHeader>
                    <CardTitle>Member Details</CardTitle>
                </CardHeader>
                <CardContent className="space-y-2">
                    <div><span className="font-semibold">Name:</span> {member?.firstName} {member?.lastName}</div>
                    <div><span className="font-semibold">Email:</span> {member?.email}</div>
                    <div><span className="font-semibold">Status:</span> {member?.status || "Active"}</div>
                    <div><span className="font-semibold">Joined:</span> {member?.joinDate ? new Date(member.joinDate).toLocaleDateString() : "N/A"}</div>
                </CardContent>
            </Card>
        </div>
    )
}
