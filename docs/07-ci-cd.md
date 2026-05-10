# CI/CD

El workflow esta en `.github/workflows/ci.yml`.

Pipeline incluido:

- Checkout del repositorio.
- Instalacion de .NET 10.
- Restore.
- Build en Release.
- Test con cobertura usando Coverlet.
- Publicacion del archivo Cobertura como artifact.

Practicas recomendadas:

- Mantener CI rapido.
- Separar performance tests de la ejecucion comun.
- Fallar el pipeline ante regresiones de pruebas.
- Revisar cobertura como senal, no como objetivo unico.
