'use client';

import { Customer } from '@/types';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { useRouter } from 'next/navigation';
import { ArrowLeft } from 'lucide-react';
import { format } from 'date-fns';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';

// Helper component for displaying a detail item
function DetailItem({ label, value, isTruncated = false }: { label: string; value: React.ReactNode; isTruncated?: boolean }) {
    return (
        <div>
            <p className="text-sm text-muted-foreground">{label}</p>
            <p className={`text-sm font-semibold ${isTruncated ? 'truncate' : ''}`}>{value || '-'}</p>
        </div>
    );
}

export default function MemberDetailsView({ member }: { member: Customer | null }) {
  const router = useRouter();

  if (!member) {
    return (
      <div>
        <Button variant="outline" onClick={() => router.push('/members/list')}>
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to List
        </Button>
        <p className="mt-4 text-destructive">Member not found or failed to load.</p>
      </div>
    );
  }

  return (
    // Main container using a robust flexbox layout
    <div className="flex flex-col lg:flex-row gap-8 w-full">
      
        {/* Left Column (Fixed Width on large screens) */}
        <aside className="lg:w-80 lg:flex-shrink-0">
            <Card>
                <CardContent className="p-6">
                    <div className="flex flex-col items-center text-center space-y-4">
                        <Avatar className="h-24 w-24">
                            <AvatarImage src={`https://api.dicebear.com/7.x/initials/svg?seed=${member.firstName} ${member.lastName}`} />
                            <AvatarFallback>{member.firstName[0]}{member.lastName[0]}</AvatarFallback>
                        </Avatar>
                        <div>
                            <h2 className="text-2xl font-bold">{member.firstName} {member.lastName}</h2>
                            <Badge variant={member.isActive ? 'default' : 'destructive'} className="mt-1">
                                {member.isActive ? 'Active' : 'Inactive'}
                            </Badge>
                        </div>
                    </div>

                    <Separator className="my-6" />

                    <div className="space-y-4">
                        <DetailItem label="Email" value={member.email} />
                        <DetailItem label="Referral Token" value={member.referralCode} />
                        <DetailItem label="Member ID" value={member.id} isTruncated={true} />
                        <DetailItem label="Phone Number" value={member.phoneNumber} />
                        <DetailItem label="Loyalty Card No" value={member.loyaltyCardNumber} />
                        <DetailItem label="Tier" value={member.loyaltyTierName} />
                    </div>
                </CardContent>
            </Card>
        </aside>

        {/* Right Column (Takes remaining space) */}
        <main className="flex-1">
            <Card>
                <CardHeader><CardTitle>Timeline</CardTitle></CardHeader>
                <CardContent>
                    <p>Future content for member timeline, transactions, etc., will be displayed here.</p>
                </CardContent>
            </Card>
        </main>

    </div>
  );
}
