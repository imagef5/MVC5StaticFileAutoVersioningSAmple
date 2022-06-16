# MVC5 js , css 파일 Auto Versioning
.NET MVC 5 js , css 파일 수정사항 발생시 자동으로 버전 변경 하기

-예)  <script src="@Url.AppendVersion("~/Scripts/jquery.validate.min.js")"></script>

```
        /// <summary>
        /// 파일명 + 파일 버전 설정하기
        /// </summary>
        /// <param name="contentPath">파일 상대경로</param>
        /// <returns>파일명 + 파일 버전(예: /Content/js/commonjs?v=DcG3GupjuUbYM9tq2ZM_OF0nMg06aSLT4cry2Mb4b68)</returns>
        /// <remarks>
        ///     동일서버 javascript , css 또는 정적 파일에만 적용할것
        ///     리모트서버의 파일에는 사용금지
        /// </remarks>
        public static string AppendVersion(this UrlHelper helper, string contentPath)
        {

            bool isParam = false;
            string paramString = string.Empty;
            if (contentPath.Contains("?"))
            {
                isParam = true;
                var path = contentPath.Split('?');
                contentPath = path[0];
                paramString = "?" + path[1];
            }

            contentPath = helper.Content(contentPath);
            var version = GetFileVersion(contentPath, contentPath, helper.RequestContext.HttpContext);
            version = isParam ? paramString + "&v=" + version : "?v=" + version;

            return contentPath + version;
        }

        /// <summary>
        /// 파일 버전 설정 및 값 리턴
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="contentPath">파일명(경로포함)</param>
        /// <returns>version string (예 : DcG3GupjuUbYM9tq2ZM_OF0nMg06aSLT4cry2Mb4b68)</returns>
        /// <remarks>
        ///     동일서버 javascript , css 또는 정적 파일에만 적용할것
        ///     리모트서버의 파일에는 사용금지
        /// </remarks>
        private static string GetFileVersion(string key, string contentPath, HttpContextBase context)
        {
            var cache = MemoryCache.Default;
            var versionItem = cache.GetCacheItem(key);

            if (versionItem == null)
            {
                var filePath = context.Server.MapPath(contentPath);

                if (File.Exists(filePath))
                {
                    //캐시 정책 설정
                    CacheItemPolicy policy = new CacheItemPolicy();
                    //모니터링할 파일및 폴더 목록 설정
                    List<string> cachedFilePaths = new List<string>();
                    cachedFilePaths.Add(filePath);
                    //파일 변경 모니터링 설정 : 파일(or 폴더) 수정 및 삭제시 캐시 무효화 처리 및
                    //1시간 마다 캐시 제거 병행 적용
                    policy.ChangeMonitors.Add(new HostFileChangeMonitor(cachedFilePaths));
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60);
                    //캐시 아이템 설정
                    string version = GetHashForFile(filePath);
                    versionItem = new CacheItem(key, version);
                    //캐시 설정
                    cache.Set(versionItem, policy);
                }
                else
                {
                    ////파일이 존재하지 않는경우
                    return string.Empty;
                }
            }

            return versionItem.Value.ToString();
        }

        /// <summary>
        /// 파일 해시값 가져오기
        /// </summary>
        /// <param name="filePath">물리적 파일 경로명</param>
        /// <returns>Base64UrlEncoded 된 hash 값</returns>
        private static string GetHashForFile(string filePath)
        {
            using (var sha = SHA256.Create())
            using (var readStream = File.OpenRead(filePath))
            {
                var hash = sha.ComputeHash(readStream);
                return Convert.ToBase64String(hash)
                                .Replace('+', '-')
                                .Replace('/', '_')
                                .Replace("=", "");
            }
        }

```
