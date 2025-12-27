'use client';

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger, DropdownMenuSeparator } from "@/components/ui/dropdown-menu";
import { IconDotsVertical, IconFilter, IconPlus, IconSearch } from "@tabler/icons-react";
import { Input } from "@/components/ui/input";
import Link from "next/link";

// Mock data for the coupon definitions list
const coupons = [
  {
    id: 'coup_1',
    name: 'New Year 10% Off',
    code: 'NY2025',
    discount: '10%',
    applicability: 'Order',
    codeType: 'Global',
    status: 'ACTIVE',
    validity: '25 Dec 2024 - 10 Jan 2025',
    usage: '152 / 1000',
  },
  {
    id: 'coup_2',
    name: 'Welcome Bonus - ₹250',
    code: '-',
    discount: '₹250',
    applicability: 'Order',
    codeType: 'Unique',
    status: 'ACTIVE',
    validity: '-',
    usage: '89 / ∞',
  },
  {
    id: 'coup_3',
    name: 'Free Shipping Weekend',
    code: 'FREESHIP',
    discount: 'Free Shipping',
    applicability: 'Shipping',
    codeType: 'Global',
    status: 'PAUSED',
    validity: 'Every Weekend',
    usage: '1,204 / 5000',
  },
    {
    id: 'coup_4',
    name: 'Diwali Special',
    code: 'DIWALI24',
    discount: '15%',
    applicability: 'Order',
    codeType: 'Global',
    status: 'EXPIRED',
    validity: '01 Nov 2024 - 05 Nov 2024',
    usage: '987 / 1000',
  },
    {
    id: 'coup_5',
    name: 'Internal Test Coupon',
    code: 'TEST50',
    discount: '50%',
    applicability: 'Order',
    codeType: 'Global',
    status: 'DRAFT',
    validity: '-',
    usage: '0 / 10',
  },
];

export default function CouponsPage() {
  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Coupon Management</h1>
        <div className="flex gap-2">
          <Link href="/coupons/create">
            <Button>
              <IconPlus className="mr-2 h-4 w-4" />
              New Coupon
            </Button>
          </Link>
        </div>
      </div>

       <div className="flex justify-between items-center mb-4 gap-2">
            <div className="relative w-full max-w-sm">
                <IconSearch className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                <Input placeholder="Search by name or code..." className="pl-8" />
            </div>
            <Button variant="outline">
                <IconFilter className="mr-2 h-4 w-4" />
                Filter
            </Button>
       </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Code</TableHead>
              <TableHead>Discount</TableHead>
              <TableHead>Applicability</TableHead>
              <TableHead>Code Type</TableHead>
              <TableHead>Issued / Used</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="w-[50px]"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {coupons.map((coupon) => (
              <TableRow key={coupon.id} className="hover:bg-muted/50">
                <TableCell className="font-medium">{coupon.name}</TableCell>
                <TableCell>{coupon.code}</TableCell>
                <TableCell>{coupon.discount}</TableCell>
                <TableCell>{coupon.applicability}</TableCell>
                <TableCell><Badge variant={coupon.codeType === 'Global' ? 'outline' : 'default'}>{coupon.codeType}</Badge></TableCell>
                <TableCell>{coupon.usage}</TableCell>
                <TableCell>
                  <Badge
                    variant={
                      coupon.status === 'ACTIVE' ? 'success' : 
                      coupon.status === 'DRAFT' ? 'secondary' : 
                      coupon.status === 'PAUSED' ? 'default' : 'destructive' // Corrected: 'warning' is not a valid variant, using 'default' for PAUSED
                    }
                  >
                    {coupon.status}
                  </Badge>
                </TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="h-8 w-8 p-0">
                        <span className="sr-only">Open menu</span>
                        <IconDotsVertical className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem>View Usage</DropdownMenuItem>
                      <DropdownMenuItem>Edit</DropdownMenuItem>
                      <DropdownMenuItem>Duplicate</DropdownMenuItem>
                       <DropdownMenuItem>{coupon.status === 'ACTIVE' ? 'Pause' : 'Activate'}</DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem className="text-red-600 hover:!text-red-600 hover:!bg-red-100">Delete</DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
