// Généré par QA Platform. Valider cette proposition par pull request avant fusion.
pipeline {
    agent any

    options {
        timestamps()
        disableConcurrentBuilds()
    }

    environment {
        PATH = "/var/jenkins_home/.dotnet/tools:${env.PATH}"
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = '1'
    }

    stages {
        stage('Checkout') {
            steps { checkout scm }
        }

        stage('Install QA tools') {
            steps {
                sh 'dotnet tool update --global dotnet-sonarscanner || dotnet tool install --global dotnet-sonarscanner'
            }
        }

        stage('Restore') {
            steps { sh 'dotnet restore -p:NuGetAudit=false' }
        }

        stage('SonarQube begin') {
            steps {
                withSonarQubeEnv('SonarQubeLocal') {
                    withCredentials([string(credentialsId: 'SONAR_TOKEN', variable: 'SONAR_TOKEN')]) {
                        sh '''
                            dotnet sonarscanner begin \
                                /k:"hamzariad_GestionInterventionsDemo" \
                                /d:sonar.token="$SONAR_TOKEN" \
                                /d:sonar.cs.opencover.reportsPaths="coverage/**/coverage.opencover.xml" \
                                /d:sonar.exclusions="**/bin/**,**/obj/**,**/TestResults/**"
                        '''
                    }
                }
            }
        }

        stage('Build') {
            steps { sh 'dotnet build --configuration Release --no-restore' }
        }

        stage('Automated tests') {
            steps {
                sh '''
                    mkdir -p TestResults coverage
                    test_projects=$(find tests -name "*.csproj" -type f 2>/dev/null || true)
                    if [ -z "$test_projects" ]; then
                        echo "Aucun projet de tests automatisés détecté."
                        exit 0
                    fi
                    e2e_projects=$(find tests -name "*E2E*.csproj" -type f 2>/dev/null || true)
                    app_pid=""
                    if [ -n "$e2e_projects" ]; then
                        web_project=$(find . -name "*.csproj" -not -path "./tests/*" -exec grep -l 'Microsoft.NET.Sdk.Web' {} + | head -n 1)
                        if [ -z "$web_project" ]; then
                            echo "Projet ASP.NET Core introuvable pour les tests E2E."
                            exit 1
                        fi
                        ASPNETCORE_URLS=http://127.0.0.1:5080 dotnet run --project "$web_project" --configuration Release --no-build --no-launch-profile -- --urls http://127.0.0.1:5080 > qa-platform-e2e-server.log 2>&1 &
                        app_pid=$!
                        trap 'kill $app_pid 2>/dev/null || true' EXIT
                        app_ready=0
                        for attempt in $(seq 1 30); do
                            if curl --silent --output /dev/null --max-time 2 http://127.0.0.1:5080/; then app_ready=1; break; fi
                            if ! kill -0 $app_pid 2>/dev/null; then cat qa-platform-e2e-server.log; exit 1; fi
                            sleep 1
                        done
                        if [ "$app_ready" -ne 1 ]; then
                            echo "L'application E2E n'est pas accessible sur http://127.0.0.1:5080 après 30 secondes."
                            cat qa-platform-e2e-server.log
                            exit 1
                        fi
                        export E2E_BASE_URL=http://127.0.0.1:5080
                    fi
                    for project in $test_projects; do
                        project_name=$(basename "$project" .csproj)
                        dotnet test "$project" --configuration Release \
                            --logger "trx;LogFileName=$project_name.trx" \
                            --results-directory "$WORKSPACE/TestResults/$project_name" \
                            /p:CollectCoverage=true \
                            /p:CoverletOutputFormat=opencover \
                            /p:CoverletOutput="$WORKSPACE/coverage/$project_name/coverage.opencover.xml"
                    done
                    if [ -n "$app_pid" ]; then kill $app_pid 2>/dev/null || true; fi
                '''
            }
        }

        stage('SonarQube end') {
            steps {
                withSonarQubeEnv('SonarQubeLocal') {
                    withCredentials([string(credentialsId: 'SONAR_TOKEN', variable: 'SONAR_TOKEN')]) {
                        sh 'dotnet sonarscanner end /d:sonar.token="$SONAR_TOKEN"'
                    }
                }
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: 'TestResults/**/*.trx, coverage/**/*.xml', allowEmptyArchive: true, fingerprint: true
        }
    }
}
