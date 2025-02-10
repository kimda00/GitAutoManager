using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: GitAutoManager <commit message>");
            return;
        }

        // Console.WriteLine("자동 커밋 테스트 중입니다!");


        string commitMessage = args[0];

        // 1. 변경 사항 확인 (git diff)
        Console.WriteLine("🔍 Checking for changes...");
        RunGitCommand("diff --name-only");

        // 2. 변경 사항 추가 및 커밋
        RunGitCommand("add .");
        RunGitCommand($"commit -m \"{commitMessage}\"");

        // 3. 최신 내용 Pull
        Console.WriteLine("⬇️ Pulling latest changes...");
        RunGitCommand("pull");

        // 4. 충돌 여부 확인
        if (HasMergeConflicts())
        {
            Console.WriteLine("❌ Merge conflicts detected! Please resolve them manually.");
            return;
        }

        // 5. 자동 푸시
        Console.WriteLine("⬆️ Pushing changes to remote repository...");
        RunGitCommand("push");

        Console.WriteLine("✅ All operations completed successfully!");
    }

    static void RunGitCommand(string command)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "git", // 실행 할 프로그램
            Arguments = command, // 실행 할 명령어
            RedirectStandardOutput = true, // git 명령어의 출력 결과 가져오기
            RedirectStandardError = true, // 오류 메시지 가져오기
            UseShellExecute = false, // 쉘 사용 않고 직접 실행
            CreateNoWindow = true // 별도 콘솔창 x 
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine(output);
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"❌ Error executing 'git {command}': {error}");
            }
        }
    }

    static bool HasMergeConflicts()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "ls-files -u",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return !string.IsNullOrEmpty(output);
        }
    }
}
