using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Repository;

public class AcademicRepository(AcademicAppContext context, IMapper mapper) : IAcademicRepository
{
    private readonly AcademicAppContext _context = context;
	private readonly IMapper _mapper = mapper;

	public async Task<EnrollmentResponseDTO> AddEnrollmentAsync(EnrollmentPostDTO enrollment)
    {
        var entity = _mapper.Map<Enrollment>(enrollment);
		await _context.Enrollments.AddAsync(entity);

        return _mapper.Map<EnrollmentResponseDTO>(entity);
    }

    public async Task<FacultyResponseDTO> AddFacultyAsync(FacultyPostDTO faculty)
    {
		var entity = _mapper.Map<Faculty>(faculty);
		await _context.Faculties.AddAsync(entity);

        return _mapper.Map<FacultyResponseDTO>(entity);
    }

    public async Task<StudentGroupResponseDTO> AddGroupAsync(StudentGroupPostDTO studentGroup)
    {
		var entity = _mapper.Map<StudentGroup>(studentGroup);
		await _context.Groups.AddAsync(entity);

        return _mapper.Map<StudentGroupResponseDTO>(entity);
	}

    public async Task<PromotionResponseDTO> AddPromotionAsync(PromotionPostDTO promotion)
    {
		var entity = _mapper.Map<Promotion>(promotion);
		await _context.Promotions.AddAsync(entity);

        return _mapper.Map<PromotionResponseDTO>(entity);
	}

    public async Task<SpecialisationResponseDTO> AddSpecialisationAsync(SpecialisationPostDTO specialisation)
    {
		var entity = _mapper.Map<Specialisation>(specialisation);
		await _context.Specialisations.AddAsync(entity);

		return _mapper.Map<SpecialisationResponseDTO>(entity);
	}

    public async Task<StudentSubGroupResponseDTO> AddSubGroupAsync(StudentSubGroupPostDTO studentSubGroup)
    {
		var entity = _mapper.Map<StudentSubGroup>(studentSubGroup);
		await _context.SubGroups.AddAsync(entity);

		return _mapper.Map<StudentSubGroupResponseDTO>(entity);
	}

    public async Task<TeacherResponseDTO> AddTeacherAsync(TeacherPostDTO teacher)
    {
		var entity = _mapper.Map<Teacher>(teacher);
		await _context.Teachers.AddAsync(entity);

		return _mapper.Map<TeacherResponseDTO>(entity);
	}

    public async Task<TeacherResponseDTO?> GetTeacherById(int id)
    {
        var teacher = await _context.Teachers
            .Include(t => t.User)
            .Include(t => t.Faculty)
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync();

        return _mapper.Map<TeacherResponseDTO>(teacher);
    }

    public async Task<List<EnrollmentResponseDTO>> GetEnrollmentsByUserId(int userId)
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.SubGroup)
                .ThenInclude(sg => sg.StudentGroup)
                    .ThenInclude(g => g.Promotion)
                        .ThenInclude(gy => gy.Specialisation)
                            .ThenInclude(s => s.Faculty)
            .Include(e => e.User)
            .Where(e => e.UserId == userId)
            .ToListAsync();

		return _mapper.Map<List<EnrollmentResponseDTO>>(enrollments);
	}

    public async Task<FacultyResponseDTO?> GetFacultyByIdAsync(int id)
    {
        var faculty = await _context.Faculties
            .Include(f => f.Specialisations)
            .SingleOrDefaultAsync(f => f.Id == id);

		return _mapper.Map<FacultyResponseDTO>(faculty);
	}

    public async Task<FacultyResponseDTO?> GetFacultyByNameAsync(string name)
    {
        var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Name == name);

		return _mapper.Map<FacultyResponseDTO>(faculty);
	}

    public async Task<StudentGroupResponseDTO?> GetGroupByIdAsync(int id)
    {
        var studentGroup = await _context.Groups
            .Include(g => g.StudentSubGroups)
            .Include(g => g.Promotion)
                .ThenInclude(gy => gy.Specialisation)
                    .ThenInclude(s => s.Faculty)
            .FirstOrDefaultAsync(g => g.Id == id);

		return _mapper.Map<StudentGroupResponseDTO>(studentGroup);
	}

    public async Task<PromotionResponseDTO?> GetPromotionByIdAsync(int id)
    {
        var promotion = await _context.Promotions
            .Include(gy => gy.StudentGroups)
            .Include(gy => gy.Specialisation)
                .ThenInclude(s => s.Faculty)
            .FirstOrDefaultAsync(gy => gy.Id == id);

		return _mapper.Map<PromotionResponseDTO>(promotion);
	}

    public async Task<SpecialisationResponseDTO?> GetSpecialisationByIdAsync(int id)
    {
        var specialisation = await _context.Specialisations
             .Include(s => s.Faculty)
             .Include(s => s.Promotions)
             .FirstOrDefaultAsync(s => s.Id == id);

		return _mapper.Map<SpecialisationResponseDTO>(specialisation);
	}

	public async Task<StudentSubGroupResponseDTO?> GetSubGroupByIdAsync(int id)
    {
        var studentSubGroup = await _context.SubGroups
            .Include(sg => sg.StudentGroup)
                .ThenInclude(g => g.Promotion)
                    .ThenInclude(gy => gy.Specialisation)
                        .ThenInclude(s => s.Faculty)
            .Include(sg => sg.Enrollments)
            .FirstOrDefaultAsync(sg => sg.Id == id);

		return _mapper.Map<StudentSubGroupResponseDTO>(studentSubGroup);
	}

	public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<EnrollmentResponseDTO?> GetEnrollmentByIdAsync(int enrollmentId)
    {
       var enrollment = await _context.Enrollments.Where(e => e.Id == enrollmentId)
            .Include(e => e.User)
            .Include(e => e.SubGroup)
                .ThenInclude(sg => sg.StudentGroup)
                    .ThenInclude(g => g.Promotion)
                        .ThenInclude(gy => gy.Specialisation)
                            .ThenInclude(s => s.Faculty)
            .FirstOrDefaultAsync();
        return _mapper.Map<EnrollmentResponseDTO>(enrollment);
	}
}
