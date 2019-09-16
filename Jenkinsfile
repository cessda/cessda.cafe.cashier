pipeline {
	options {
		ansiColor('xterm')
		buildDiscarder logRotator(artifactNumToKeepStr: '5', numToKeepStr: '10')
	}

	environment {
		project_name = "${GCP_PROJECT}"
		product_name = "cafe"
		module_name = "cashier"
		image_tag = "${docker_repo}/${product_name}-${module_name}:${env.BRANCH_NAME}-${env.BUILD_NUMBER}"
		HOME = '/tmp'
		scannerHome = tool 'sonar-scanner-msbuild'
	}

	agent any

	stages {
		stage('Pull SDK Docker Image') {
			agent {
				docker {
					image 'dotnet/core/sdk:2.2-stretch'
					registryUrl 'https://mcr.microsoft.com'
					reuseNode true
				}
			}
			stages {
				stage('Build Cashier') {
					steps {
						withSonarQubeEnv('cessda-sonar') {
							sh "dotnet ${scannerHome} begin /k:\"eu.cessda.cafe:cashier\""
							sh 'dotnet build -c Release'
						}
					}
					post {
						always {
							archiveArtifacts 'Cashier/bin/Release/netcoreapp2.2/**'
							recordIssues(tools: [msBuild()])
						}
					}
				}
				stage('Test Cashier') {
					steps {
						sh 'dotnet test --logger:trx --no-build'
					}
					post {
						always {
							mstest failOnError: false
						}
					}
				}
				stage('Run Sonar Scan') {
					steps {
						withSonarQubeEnv('cessda-sonar') {
							sh 'dotnet ${scannerHome} end'
						}
					}
					when { branch 'master' }
				}
				stage('Get Quality Gate Status') {
					steps {
						timeout(time: 1, unit: 'HOURS') {
							waitForQualityGate abortPipeline: true
						}
					}
					when { branch 'master' }
				}
				stage('Publish Cashier') {
					steps {
						sh 'dotnet publish --no-build -c Release -o ./publish'
					}
					when { branch 'master' }
				}
			}
		}
		stage('Build Docker Container') {
			steps {
				sh "docker build -t ${image_tag} ."
			}
			when { branch 'master' }
		}
		stage('Push Docker Container') {
			steps {
				sh("gcloud auth configure-docker")
				sh("docker push ${image_tag}")
				sh("gcloud container images add-tag ${image_tag} ${docker_repo}/${product_name}-${module_name}:${env.BRANCH_NAME}-latest")
			}
			when { branch 'master' }
		}
	}
}