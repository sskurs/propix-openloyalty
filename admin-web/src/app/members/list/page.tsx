'use client';

import { useEffect, useState } from 'react';
import { MemberListItem } from '@/types';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Card, CardContent, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useRouter } from 'next/navigation';
import { toast } from 'sonner';
import { Badge } from "@/components/ui/badge";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Filter, PlusCircle, UserCog } from "lucide-react";
import { DotsHorizontalIcon } from "@radix-ui/react-icons"; 
import { Input } from '@/components/ui/input';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { getMembers, toggleMemberStatus } from '@/api/membersApi'; // Updated import

export default function MemberListPage() {
  const [members, setMembers] = useState<MemberListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  const loadMembers = async () => {
    setLoading(true);
    try {
      const fetchedMembers = await getMembers();
      setMembers(fetchedMembers);
    } catch (error: any) {
      toast.error(error.message || "Failed to load members.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadMembers();
  }, []);

  const handleToggleStatus = (memberId: string, currentStatus: string) => {
    const newStatus = currentStatus === 'active';
    const promise = toggleMemberStatus(memberId, newStatus).then(() => loadMembers());

    toast.promise(promise, {
        loading: 'Updating status...',
        success: 'Member status has been updated.',
        error: (err) => err.message || 'Failed to update status.',
    });
  };

  return (
    // The root is now the flex container for the whole page view
    <div className="flex flex-col h-full">
        {/* Sticky header */}
        <header className="bg-card/95 backdrop-blur-sm p-4 border-b">
          <div className="flex items-center justify-between gap-4">
              <h1 className="text-2xl font-bold">Members</h1>
              <div className="flex items-center gap-2">
                  <Input placeholder="Search members..." className="max-w-sm" />
                  <Button variant="outline"><Filter className="mr-2 h-4 w-4" /> Filters</Button>
                  <Button variant="outline"><UserCog className="mr-2 h-4 w-4" /> Bulk Actions</Button>
                  <Button onClick={() => router.push('/members/create')}><PlusCircle className="mr-2 h-4 w-4"/> Add Member</Button>
              </div>
          </div>
        </header>

      {/* Scrolling Content Area */}
      <main className="flex-1 overflow-y-auto p-4">
          {loading ? (
              <p>Loading...</p>
          ) : (
            <Card>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Name</TableHead>
                                <TableHead>Email</TableHead>
                                <TableHead>Tier</TableHead>
                                <TableHead>Points</TableHead>
                                <TableHead>Status</TableHead>
                                <TableHead>Joined</TableHead>
                                <TableHead><span className="sr-only">Actions</span></TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                        {members.map((member) => (
                            <TableRow key={member.id}>
                                <TableCell className="font-medium flex items-center gap-3">
                                    <Avatar>
                                        <AvatarImage src={`https://api.dicebear.com/7.x/initials/svg?seed=${member.firstName} ${member.lastName}`} />
                                        <AvatarFallback>{member.firstName?.[0]}{member.lastName?.[0]}</AvatarFallback>
                                    </Avatar>
                                    <div>
                                        <div>{member.firstName} {member.lastName}</div>
                                        <div className="text-xs text-muted-foreground">ID: {member.id.substring(0, 8)}...</div>
                                    </div>
                                </TableCell>
                                <TableCell>{member.email}</TableCell>
                                <TableCell><Badge variant="secondary">{member.tierName || 'N/A'}</Badge></TableCell>
                                <TableCell>{member.pointsBalance.toLocaleString()}</TableCell>
                                <TableCell><Badge variant={member.status === 'active' ? 'default' : 'destructive'}>{member.status}</Badge></TableCell>
                                <TableCell>{new Date(member.joinDate).toLocaleDateString()}</TableCell>
                                <TableCell>
                                    <DropdownMenu>
                                        <DropdownMenuTrigger asChild>
                                            <Button variant="ghost" className="h-8 w-8 p-0">
                                                <span className="sr-only">Open menu</span>
                                                <DotsHorizontalIcon className="h-4 w-4" />
                                            </Button>
                                        </DropdownMenuTrigger>
                                        <DropdownMenuContent align="end">
                                            <DropdownMenuItem onClick={() => router.push(`/members/${member.id}`)}>View Profile</DropdownMenuItem>
                                            <DropdownMenuItem onClick={() => router.push(`/members/${member.id}/edit`)}>Edit Profile</DropdownMenuItem>
                                            <DropdownMenuItem>Add Tag</DropdownMenuItem>
                                            <DropdownMenuItem onClick={() => handleToggleStatus(member.id, member.status)} className={member.status !== 'active' ? 'text-destructive' : ''}>
                                                {member.status === 'active' ? 'Deactivate' : 'Activate'}
                                            </DropdownMenuItem>
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        ))}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>
          )}
      </main>
    </div>
  );
}
