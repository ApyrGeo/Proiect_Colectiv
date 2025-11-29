using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TrackForUBB.Repository;
using TrackForUBB.Repository.AutoMapper;
using TrackForUBB.Repository.Context;

namespace TrackForUBB.BackendTests;

public class GradeRepositoryTests : IDisposable
{
    private readonly AcademicAppContext _context;
    private readonly GradeRepository _repo;

    public GradeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AcademicAppContext>()
            .UseInMemoryDatabase(databaseName: "GradeRepositoryTestsDB")
            .Options;
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<EFEntitiesMappingProfile>(); },
            new NullLoggerFactory());

        IMapper mapper = config.CreateMapper();
        _context = new AcademicAppContext(options);
        _repo = new GradeRepository(_context, mapper);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    
}