pipeline {
	environment {
		project_name = "${GCP_PROJECT}"
		product_name = "cafe"
		module_name = "cashier"
		image_tag = "${DOCKER_ARTIFACT_REGISTRY}/${product_name}-${module_name}:${env.BUILD_NUMBER}"
		HOME = "/tmp"
		build_configuration = "Release"
		version = "1.8.0"
	}

	agent any

	stages {
		stage('Pull SDK Docker Image') {
			agent {
				dockerfile {
					dir 'CI'
					filename 'build.Dockerfile'
					reuseNode true
				}
			}
			stages {
				stage('Start SonarQube') {
					steps {
						withSonarQubeEnv('cessda-sonar') {
							sh "dotnet tool install --global dotnet-sonarscanner"
							sh("export PATH=\"$PATH:/tmp/.dotnet/tools\" && dotnet sonarscanner begin " + 
							"/k:'eu.cessda.cafe:cashier' /v:${version}.${env.BUILD_NUMBER}  /n:'CESSDA Café: Cashier' " +
							"/d:'sonar.cs.opencover.reportsPaths=Cashier.Tests/coverage.opencover.xml' " +
							"/d:'sonar.projectDescription=Cashier implementation of the CESSDA Coffee API' " +
							"/d:'sonar.links.ci=https://jenkins.cessda.eu/' " +
							"/d:'sonar.links.scm=https://bitbucket.org/cessda/cessda.cafe.cashier/'")
						}
					}
					when { branch 'master' }
				}
				stage('Build Cashier') {
					steps {
						sh "dotnet build -c ${build_configuration} /p:Version=${version}.${env.BUILD_NUMBER}"
					}					
					post {
						always {
							archiveArtifacts 'Cashier/bin/Release/**'
							recordIssues(tools: [msBuild()])
						}
					}
				}
				stage('Test Cashier') {
					steps {
						sh "dotnet test -c ${build_configuration} /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --logger:trx --no-build"
					}
					post {
						always {
							mstest()
						}
					}
				}
				stage('Run Sonar Scan') {
					steps {
						withSonarQubeEnv('cessda-sonar') {
							sh "export PATH=\"$PATH:/tmp/.dotnet/tools\" && dotnet sonarscanner end"
						}
						timeout(time: 1, unit: 'HOURS') {
							waitForQualityGate abortPipeline: false
						}
					}
					when { branch 'master' }
				}
				stage('Publish Cashier') {
					steps {
						sh "dotnet publish \"Cashier/Cashier.csproj\" --no-build -c ${build_configuration} -o ./publish"
					}
					when { branch 'master' }
				}
			}
		}
		stage('Build Docker Container') {
			steps {
				sh "docker build --file=CI/publish.Dockerfile --tag=${image_tag} ."
			}
			when { branch 'master' }
		}
		stage('Push Docker Container') {
			steps {
				sh "gcloud auth configure-docker ${ARTIFACT_REGISTRY_HOST}"
				sh "docker push ${image_tag}"
				sh "gcloud artifacts docker tags add ${image_tag} ${DOCKER_ARTIFACT_REGISTRY}/${product_name}-${module_name}:latest"
			}
			when { branch 'master' }
		}
        stage('Deploy Docker image'){
            steps{
                build job: '../cessda.cafe.deployment/master', parameters: [string(name: 'image_tag', value: "${env.BRANCH_NAME}-${env.BUILD_NUMBER}"), string(name: 'component', value: 'cashier')], wait: false
            }
			when { branch 'master' }
        }
	}
}