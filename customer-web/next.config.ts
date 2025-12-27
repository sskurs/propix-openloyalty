import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: "standalone", // ✅ ADD THIS LINE HERE

  async rewrites() {
    return [
      {
        source: "/api/pos/:path*",
        destination: "http://backend:8080/api/pos/:path*",
      },
      {
        source: "/api/members/:path*",
        destination: "http://backend:8080/api/members/:path*",
      },
      {
        source: "/api/:path*",
        destination: `${process.env.NEXT_PUBLIC_API_URL || "http://customer-api:8081/api"
          }/:path*`,
      },
    ];
  },
};

export default nextConfig;
