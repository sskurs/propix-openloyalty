'use client';

import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { Checkbox } from '@/components/ui/checkbox';
import { useState } from 'react';

export default function CreateCommunicationCampaignPage() {
  const [step, setStep] = useState(1);

  return (
    <div>
      <h1 className="text-3xl font-bold mb-8">Create Communication Campaign</h1>
      
      {/* Stepper UI */}
      <div className="mb-8 flex justify-center">
        <ol className="flex items-center w-full max-w-xl">
          <li className={`flex w-full items-center ${step >= 1 ? 'text-blue-600' : 'text-gray-500'} after:content-[''] after:w-full after:h-1 after:border-b ${step > 1 ? 'after:border-blue-100' : 'after:border-gray-200'} after:border-4`}>
            <span className={`flex items-center justify-center w-10 h-10 ${step >= 1 ? 'bg-blue-100' : 'bg-gray-100'} rounded-full shrink-0`}>1</span>
          </li>
          <li className={`flex w-full items-center ${step >= 2 ? 'text-blue-600' : 'text-gray-500'} after:content-[''] after:w-full after:h-1 after:border-b ${step > 2 ? 'after:border-blue-100' : 'after:border-gray-200'} after:border-4`}>
            <span className={`flex items-center justify-center w-10 h-10 ${step >= 2 ? 'bg-blue-100' : 'bg-gray-100'} rounded-full shrink-0`}>2</span>
          </li>
          <li className={`flex w-full items-center ${step >= 3 ? 'text-blue-600' : 'text-gray-500'} after:content-[''] after:w-full after:h-1 after:border-b ${step > 3 ? 'after:border-blue-100' : 'after:border-gray-200'} after:border-4`}>
            <span className={`flex items-center justify-center w-10 h-10 ${step >= 3 ? 'bg-blue-100' : 'bg-gray-100'} rounded-full shrink-0`}>3</span>
          </li>
          <li className={`flex items-center ${step >= 4 ? 'text-blue-600' : 'text-gray-500'}`}>
            <span className={`flex items-center justify-center w-10 h-10 ${step >= 4 ? 'bg-blue-100' : 'bg-gray-100'} rounded-full shrink-0`}>4</span>
          </li>
        </ol>
      </div>

      {step === 1 && (
        <Card className="max-w-2xl mx-auto">
            <CardHeader><CardTitle>Step 1: Basics</CardTitle></CardHeader>
            <CardContent className="space-y-4">
                <div className="space-y-2"><Label>Campaign Name</Label><Input placeholder="e.g., Weekend Double Points – Email" /></div>
                <div className="space-y-2"><Label>Channel</Label><Select><SelectTrigger><SelectValue placeholder="Select a channel" /></SelectTrigger><SelectContent><SelectItem value="EMAIL">EMAIL</SelectItem><SelectItem value="SMS">SMS</SelectItem></SelectContent></Select></div>
                <div className="space-y-2"><Label>Template</Label><Select><SelectTrigger><SelectValue placeholder="Select a template" /></SelectTrigger><SelectContent><SelectItem value="template-1">Weekend Offer Template</SelectItem></SelectContent></Select></div>
            </CardContent>
        </Card>
      )}

      {step === 2 && (
        <Card className="max-w-2xl mx-auto">
            <CardHeader><CardTitle>Step 2: Audience</CardTitle></CardHeader>
            <CardContent className="space-y-4">
                 <div className="space-y-2"><Label>Audience Type</Label><RadioGroup defaultValue="segments"><div className="flex gap-4"><RadioGroupItem value="segments" id="r1" /><Label htmlFor="r1">Segments</Label><RadioGroupItem value="tags" id="r2" /><Label htmlFor="r2">Tags</Label></div></RadioGroup></div>
                 <div className="space-y-2"><Label>Segments</Label><Select><SelectTrigger><SelectValue placeholder="Select segments" /></SelectTrigger><SelectContent><SelectItem value="high-value">High Value Customers</SelectItem></SelectContent></Select></div>
                 <p className="text-sm text-muted-foreground">Estimated recipients: 8,450 members</p>
            </CardContent>
        </Card>
      )}

      {step === 3 && (
          <Card className="max-w-2xl mx-auto">
            <CardHeader><CardTitle>Step 3: Schedule</CardTitle></CardHeader>
            <CardContent className="space-y-4">
                <RadioGroup defaultValue="now"><div className="flex gap-4"><RadioGroupItem value="now" id="r1" /><Label htmlFor="r1">Send now</Label><RadioGroupItem value="schedule" id="r2" /><Label htmlFor="r2">Schedule for later</Label></div></RadioGroup>
                <Input type="datetime-local" />
                <div className="flex items-center space-x-2"><Checkbox id="opt-out" defaultChecked /><Label htmlFor="opt-out">Respect communication preferences / opt-outs</Label></div>
            </CardContent>
        </Card>
      )}

      {step === 4 && (
        <Card className="max-w-2xl mx-auto">
            <CardHeader><CardTitle>Step 4: Review & Confirm</CardTitle></CardHeader>
            <CardContent>
                <p>Summary of campaign details will be displayed here.</p>
            </CardContent>
        </Card>
      )}

      <div className="flex justify-center gap-4 mt-8">
        <Button variant="outline" onClick={() => setStep(s => Math.max(1, s - 1))} disabled={step === 1}>Previous</Button>
        <Button onClick={() => setStep(s => Math.min(4, s + 1))} disabled={step === 4}>Next</Button>
        {step === 4 && <Button>Schedule Campaign</Button>}
      </div>

    </div>
  );
}
