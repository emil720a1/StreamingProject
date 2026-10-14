# Security Scanning

The project runs automated security checks through GitHub Actions.

## Checks

The security workflow validates:

- leaked secrets and credentials with Gitleaks;
- vulnerable NuGet dependencies;
- vulnerable npm dependencies;
- Docker Compose configuration;
- backend Docker image vulnerabilities;
- frontend Docker image vulnerabilities.

## Workflow triggers

Security checks run on:

- pull requests targeting `dev`;
- pushes to `dev`;
- manual workflow dispatch.

## Failure policy

The workflow currently blocks the pipeline when:

- Gitleaks detects a secret;
- npm reports high or critical vulnerabilities;
- Trivy detects a critical vulnerability with an available fix;
- Docker Compose configuration is invalid.

NuGet vulnerability output is currently used for inspection and should be reviewed in the workflow logs.

## Vulnerability handling

When a vulnerability is detected:

1. Read the package and affected version in the workflow output.
2. Check whether a patched version is available.
3. Update the dependency through Dependabot or manually.
4. Re-run the security workflow.
5. Verify that the vulnerability is resolved.
6. Document an accepted risk if the vulnerability cannot be fixed immediately.

## False positives

Do not disable a security check immediately.

For a false positive:

1. Confirm that the value is not a real secret or exploitable dependency.
2. Use the narrowest possible ignore rule.
3. Add a comment explaining why it is safe.
4. Re-check the result in a Pull Request.

Never commit real passwords, API keys, tokens, private keys, or production connection strings.

## Local validation

Run the following checks before opening a Pull Request:

```bash
git diff --cached --check

GRAFANA_ADMIN_USER=ci-admin \
GRAFANA_ADMIN_PASSWORD=ci-password-for-validation-only \
docker compose config --quiet
```
