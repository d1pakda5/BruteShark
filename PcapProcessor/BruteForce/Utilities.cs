﻿using System;

namespace BruteForce
{
    public static class Utilities
    {
        public static string ConvertToHashcatFormat(Hash hash)
        {
            var res = string.Empty;

            if (hash is HttpDigestHash)
            {
                res = ConvertToHashcatFormat(hash as HttpDigestHash);
            }
            else if (hash is CramMd5Hash)
            {
                res = ConvertToHashcatFormat(hash as CramMd5Hash);
            }
            else if (hash is NtlmHash)
            {
                res = ConvertToHashcatFormat(hash as NtlmHash);
            }
            else
            {
                throw new Exception("Hash type not supported");
            }

            return res;
        }

        public static string ConvertToHashcatFormat(HttpDigestHash httpDigestHash)
        {
            // https://hashcat.net/forum/thread-6571.html
            // $sip$***[username] *[realm] * GET *[uri_protocol] *[uri_ip] *[uri_port] *[nonce] *[clientNonce] *[nonceCount] *[qop] * MD5 *[response]
            // TODO: Test and parse protocol and algorithm if needed.
            return string.Format("$sip$***{0}*{1}*{2}**{3}**{4}*{5}*{6}*{7}*{8}*{9}",
                httpDigestHash.Username,
                httpDigestHash.Realm,
                "GET",
                httpDigestHash.Uri,
                httpDigestHash.Nonce,
                httpDigestHash.Cnonce,
                httpDigestHash.Nc,
                httpDigestHash.Qop,
                "MD5",
                httpDigestHash.Response);
        }

        public static string ConvertToHashcatFormat(CramMd5Hash cramMd5Hash)
        {
            return $"$cram_md5${cramMd5Hash.Challenge}${cramMd5Hash.HashedData}";
        }

        public static string ConvertToHashcatFormat(NtlmHash ntlmHash)
        {
            var res = string.Empty;

            // Ntlm v1
            if (ntlmHash.NtHash.Length == 24)
            {
                res = string.Format("{0}::{1}:{2}:{3}:{4}",
                    ntlmHash.User,
                    ntlmHash.Domain,
                    ntlmHash.LmHash,
                    ntlmHash.NtHash,
                    ntlmHash.Challenge);
            }
            // Ntlm v2
            else if (ntlmHash.NtHash.Length > 24)
            {
                res = string.Format("{0}::{1}:{2}:{3}:{4}",
                    ntlmHash.User,
                    ntlmHash.Domain,
                    ntlmHash.Challenge,
                    ntlmHash.NtHash.Substring(0, 32),
                    ntlmHash.NtHash.Substring(32));
            }
            else
            {
                throw new Exception("Ntlm hash has nt part shorter than 24 chars");
            }

            return res;
        }

    }
}
