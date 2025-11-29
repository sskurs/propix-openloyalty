'use client';
import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { getSegmentById, updateSegment } from '@/api/segmentsApi';
import { Segment } from '@/api/segmentsApi';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';

export default function EditSegmentPage() {
    const params = useParams();
    const router = useRouter();
    const segmentId = params.segmentId as string;
    const [segment, setSegment] = useState<Segment | null>(null);
    const { register, handleSubmit, reset } = useForm<Segment>();

    useEffect(() => {
        if (!segmentId) return;
        getSegmentById(segmentId).then(data => {
            setSegment(data);
            reset(data); // Populate the form with existing data
        });
    }, [segmentId, reset]);

    const onSubmit = async (data: Segment) => {
        const promise = updateSegment(segmentId, data);
        toast.promise(promise, {
            loading: 'Updating segment...',
            success: () => {
                router.push('/members/segments');
                return `Segment "${data.name}" has been updated.`
            },
            error: (err) => err.message || 'An unexpected error occurred.'
        });
    };

    if (!segment) return <div>Loading...</div>;

    return (
        <div>
            <h1 className="text-3xl font-bold mb-4">Edit Segment</h1>
            <form onSubmit={handleSubmit(onSubmit)}>
                <Card className="max-w-4xl mx-auto">
                    <CardHeader><CardTitle>Segment Details</CardTitle></CardHeader>
                    <CardContent className="space-y-4">
                        <div className="space-y-2">
                            <Label htmlFor="name">Segment Name</Label>
                            <Input id="name" {...register('name')} />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="description">Description</Label>
                            <Textarea id="description" {...register('description')} />
                        </div>
                        {/* The rule builder would go here, but is omitted for brevity */}
                    </CardContent>
                    <CardFooter className="flex justify-end gap-2">
                        <Button type="button" variant="secondary" onClick={() => router.back()}>Cancel</Button>
                        <Button type="submit">Save Changes</Button>
                    </CardFooter>
                </Card>
            </form>
        </div>
    );
}
