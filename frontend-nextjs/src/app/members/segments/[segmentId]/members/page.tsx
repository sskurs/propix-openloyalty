'use client';
import { useEffect, useState } from 'react';
import { getSegmentMembers } from '@/api/segmentsApi';
import { MemberListItem } from '@/types';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { useParams } from 'next/navigation';

export default function SegmentMembersPage() {
    const params = useParams();
    const segmentId = params.segmentId as string;
    const [members, setMembers] = useState<MemberListItem[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!segmentId) return;
        getSegmentMembers(segmentId).then(data => {
            setMembers(data || []);
            setLoading(false);
        });
    }, [segmentId]);

    return (
        <div>
            <h1 className="text-2xl font-bold mb-4">Segment Members</h1>
            <div className="rounded-md border">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>Email</TableHead>
                            <TableHead>Tier</TableHead>
                            <TableHead>Points</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {loading ? (
                            <TableRow><TableCell colSpan={4} className="text-center">Loading...</TableCell></TableRow>
                        ) : (
                            members.map(member => (
                                <TableRow key={member.id}>
                                    <TableCell>{member.firstName} {member.lastName}</TableCell>
                                    <TableCell>{member.email}</TableCell>
                                    <TableCell>{member.tierName}</TableCell>
                                    <TableCell>{member.pointsBalance}</TableCell>
                                </TableRow>
                            ))
                        )}
                    </TableBody>
                </Table>
            </div>
        </div>
    );
}
