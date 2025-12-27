import Image from "next/image";

export default function AuthLayout({
    children,
}: {
    children: React.ReactNode;
}) {
    return (
        <div className="w-full lg:grid lg:min-h-screen lg:grid-cols-2">
            <div className="relative hidden h-full flex-col bg-muted p-10 text-white dark:border-r lg:flex">
                <div className="absolute inset-0 bg-zinc-900" />
                <Image
                    src="/assets/login-bg.png"
                    alt="Loyalty Background"
                    fill
                    className="object-cover opacity-60"
                    priority
                />
                <div className="relative z-20 flex items-center text-2xl font-bold tracking-tight">
                    <Image
                        src="/logo.png"
                        alt="Logo"
                        width={40}
                        height={40}
                        className="mr-3 rounded-xl shadow-lg border border-white/10"
                    />
                    Open Loyalty
                </div>
                <div className="relative z-20 mt-auto">
                    <blockquote className="space-y-2">
                        <p className="text-xl font-medium leading-relaxed italic opacity-90">
                            &ldquo;Experience a loyalty program that rewards you for every interaction. Join the elite club today.&rdquo;
                        </p>
                    </blockquote>
                </div>
            </div>
            <div className="flex items-center justify-center py-12">
                <div className="mx-auto grid w-[350px] gap-6">
                    {children}
                </div>
            </div>
        </div>
    );
}
